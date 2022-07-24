using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using slocExporter.Objects;
using slocExporter.Readers;
using UnityEditor;
using UnityEngine;

namespace slocExporter {

    public static class API {

        public const uint slocVersion = 2;

        public static readonly IObjectReader DefaultReader = new Ver2Reader();

        private static readonly Dictionary<uint, IObjectReader> VersionReaders = new Dictionary<uint, IObjectReader> {
            {1, new Ver1Reader()},
            {2, new Ver2Reader()}
        };

        public static bool CreateForAll;
        public static bool SkipForAll;

        public static bool TryGetReader(uint version, out IObjectReader reader) => VersionReaders.TryGetValue(version, out reader);

        public static IObjectReader GetReader(uint version) => TryGetReader(version, out var reader) ? reader : DefaultReader;

        public static List<slocGameObject> ReadObjects(Stream stream, bool autoClose = true, Action<string, float> updateProgress = null) {
            var objects = new List<slocGameObject>();
            var binaryReader = new BinaryReader(stream);
            var version = binaryReader.ReadUInt32();
            updateProgress?.Invoke("Reading objects", 0);
            var reader = GetReader(version);
            var count = binaryReader.ReadInt32();
            var floatCount = (float) count;
            for (var i = 0; i < count; i++) {
                var obj = ReadObject(binaryReader, version, reader);
                if (!obj.IsEmpty)
                    objects.Add(obj);
                updateProgress?.Invoke($"Reading objects ({i + 1} of {count})", i / floatCount);
            }

            if (autoClose)
                binaryReader.Close();
            return objects;
        }

        public static List<slocGameObject> ReadObjectsFromFile(string path, Action<string, float> updateProgress = null) => ReadObjects(File.OpenRead(path), true, updateProgress);

        public static GameObject CreateObjects(IEnumerable<slocGameObject> objects, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) => CreateObjects(objects, out _, position, rotation, updateProgress);

        public static GameObject CreateObjects(IEnumerable<slocGameObject> objects, out int createdAmount, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) {
            var go = new GameObject {
                transform = {
                    position = position,
                    rotation = rotation,
                }
            };
            var created = 0;
            var total = objects is ICollection<slocGameObject> l ? l.Count : -1;
            var processed = 0;
            var isCountKnown = total > 0;
            var floatTotal = (float) total;
            updateProgress?.Invoke("Creating objects", isCountKnown ? 0 : -1);
            foreach (var o in objects) {
                if (o.CreateObject(go, false) != null)
                    created++;
                processed++;
                updateProgress?.Invoke($"Creating objects ({processed}{(isCountKnown ? $" of {total}" : "")})", processed / floatTotal);
            }

            createdAmount = created;
            return go;
        }

        public static GameObject CreateObjectsFromStream(Stream objects, out int spawnedAmount, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) => CreateObjects(ReadObjects(objects), out spawnedAmount, position, rotation, updateProgress);

        public static GameObject CreateObjectsFromFile(string path, out int spawnedAmount, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) => CreateObjects(ReadObjectsFromFile(path), out spawnedAmount, position, rotation, updateProgress);

        public static GameObject CreateObject(this slocGameObject obj, GameObject parent = null, bool throwOnError = true) {
            switch (obj) {
                case PrimitiveObject primitive: {
                    var toy = GameObject.CreatePrimitive(primitive.Type.ToPrimitiveType());
                    toy.SetAbsoluteTransformFrom(parent);
                    toy.SetLocalTransform(obj.Transform);
                    if (!TryGetMaterial(primitive.MaterialColor, out var mat, out var handle)) {
                        if (handle)
                            HandleNoMaterial(primitive, toy);
                        return toy;
                    }

                    toy.GetComponent<MeshRenderer>().sharedMaterial = mat;
                    return toy;
                }
                case LightObject light: {
                    var toy = new GameObject("Spotlight");
                    var lightComponent = toy.AddComponent<Light>();
                    lightComponent.color = light.LightColor;
                    lightComponent.intensity = light.Intensity;
                    lightComponent.range = light.Range;
                    lightComponent.shadows = light.Shadows ? LightShadows.Soft : LightShadows.None;
                    toy.SetAbsoluteTransformFrom(parent);
                    toy.SetLocalTransform(obj.Transform);
                    return toy;
                }
                default:
                    if (throwOnError)
                        throw new ArgumentOutOfRangeException(nameof(obj.Type), obj.Type, "Unknown object type");
                    return null;
            }
        }

        public static void HandleNoMaterial(PrimitiveObject primitive, GameObject created) {
            if (SkipForAll)
                return;
            var result = EditorUtility.DisplayDialogComplex("No Material", "No material found for color " + primitive.MaterialColor + ".\nCreate it now?", "Create", "Create for All", "Skip");
            switch (result) {
                case 0:
                case 1:
                    CreateMaterial(primitive.MaterialColor, out var mat);
                    created.GetComponent<MeshRenderer>().sharedMaterial = mat;
                    break;
                case 2:
                    if (!EditorUtility.DisplayDialog("Skip", "Do you want to skip creating materials for all objects?", "Skip only this", "Skip for All"))
                        SkipForAll = true;
                    break;
            }

            if (result == 1)
                CreateForAll = true;
        }

        public static bool TryGetMaterial(Color color, out Material material, out bool handle) {
            handle = true;
            material = AssetDatabase.FindAssets("t:material", null)
                .Select(e => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(e)))
                .FirstOrDefault(m => m.color.Equals(color));
            if (material != null)
                return true;

            if (SkipForAll || !CreateForAll) {
                handle = !CreateForAll;
                return false;
            }

            CreateMaterial(color, out material);
            return true;
        }

        private static void CreateMaterial(Color color, out Material material) {
            material = new Material(Shader.Find("Standard")) {
                color = color
            };
            AssetDatabase.CreateAsset(material, "Assets/Colors/" + $"Material-{color.ToString()}" + ".mat");
        }

        public static slocGameObject ReadObject(this BinaryReader stream, uint version = 0, IObjectReader objectReader = null) {
            objectReader ??= GetReader(version);
            return objectReader.Read(stream);
        }

        public static slocTransform ReadTransform(this BinaryReader reader) => new slocTransform {
            Position = reader.ReadVector(),
            Scale = reader.ReadVector(),
            Rotation = reader.ReadQuaternion()
        };

        public static Vector3 ReadVector(this BinaryReader reader) => new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Quaternion ReadQuaternion(this BinaryReader reader) => new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Color ReadColor(this BinaryReader reader) => new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static PrimitiveType ToPrimitiveType(this ObjectType type) => type switch {
            ObjectType.Cube => PrimitiveType.Cube,
            ObjectType.Sphere => PrimitiveType.Sphere,
            ObjectType.Capsule => PrimitiveType.Capsule,
            ObjectType.Cylinder => PrimitiveType.Cylinder,
            ObjectType.Plane => PrimitiveType.Plane,
            ObjectType.Quad => PrimitiveType.Quad,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "A non-primitive type was supplied")
        };

        public static void SetAbsoluteTransformFrom(this GameObject component, GameObject parent) {
            if (parent != null)
                component.transform.SetParent(parent.transform, false);
        }

        public static void SetLocalTransform(this GameObject component, slocTransform transform) {
            if (component == null)
                return;
            var t = component.transform;
            t.localPosition = transform.Position;
            t.localScale = transform.Scale;
            t.localRotation = transform.Rotation;
        }

        public static string AppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string ToFullAppDataPath(this string path) => path.Replace("%appdata%", AppData);

        public static IEnumerable<GameObject> WithAllChildren(this GameObject o) => o.GetComponentsInChildren<Transform>().Select(e => e.gameObject);

    }

}
