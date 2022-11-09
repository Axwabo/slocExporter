using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using slocExporter.Objects;
using slocExporter.Readers;
using UnityEditor;
using UnityEngine;

namespace slocExporter {

    public static class API {

        public const float ColorDivisionMultiplier = 1f / 255f;

        public const ushort slocVersion = 3;

        #region Reader Declarations

        public static readonly IObjectReader DefaultReader = new Ver3Reader();

        private static readonly Dictionary<ushort, IObjectReader> VersionReaders = new() {
            {1, new Ver1Reader()},
            {2, new Ver2Reader()},
            {3, new Ver3Reader()}
        };

        public static bool TryGetReader(ushort version, out IObjectReader reader) => VersionReaders.TryGetValue(version, out reader);

        public static IObjectReader GetReader(ushort version) => TryGetReader(version, out var reader) ? reader : DefaultReader;

        #endregion

        #region Read

        private static readonly FieldInfo ReadPosField = typeof(BinaryReader).GetField("_readPos", BindingFlags.NonPublic | BindingFlags.Instance);

        // starting from v3, the version is only a ushort instead of a uint
        private static ushort ReadVersionSafe(BufferedStream buffered, BinaryReader binaryReader) {
            var newVersion = binaryReader.ReadUInt16();
            var oldVersion = binaryReader.ReadUInt16();
            if (oldVersion is not 0)
                return (ushort) (newVersion | (uint) oldVersion << 16);
            ReadPosField.SetValue(buffered, (int) ReadPosField.GetValue(buffered) - sizeof(ushort)); // rewind the buffer by two bytes, so the whole stream won't be malformed data
            return newVersion;
        }

        public static List<slocGameObject> ReadObjects(Stream stream, bool autoClose = true, Action<string, float> updateProgress = null) {
            var objects = new List<slocGameObject>();
            using var buffered = new BufferedStream(stream, 4);
            var binaryReader = new BinaryReader(buffered);
            var version = ReadVersionSafe(buffered, binaryReader);
            updateProgress?.Invoke("Reading objects", 0);
            var reader = GetReader(version);
            var header = reader.ReadHeader(binaryReader);
            var count = header.ObjectCount;
            var floatCount = (float) count;
            for (var i = 0; i < count; i++) {
                var obj = ReadObject(binaryReader, header, version, reader);
                if (obj is {IsValid: true})
                    objects.Add(obj);
                updateProgress?.Invoke($"Reading objects ({i + 1} of {count})", (i + 1) / floatCount);
            }

            if (autoClose)
                binaryReader.Close();
            return objects;
        }

        public static List<slocGameObject> ReadObjectsFromFile(string path, Action<string, float> updateProgress = null) => ReadObjects(File.OpenRead(path), true, updateProgress);

        #endregion

        #region Create

        public static GameObject CreateObject(this slocGameObject obj, GameObject parent = null, bool throwOnError = true) => obj switch {
            PrimitiveObject primitive => CreatePrimitive(obj, parent, primitive),
            LightObject light => CreateLight(obj, parent, light),
            EmptyObject => CreateEmpty(obj, parent),
            _ => throwOnError ? throw new ArgumentOutOfRangeException(nameof(obj.Type), obj.Type, "Unknown object type") : null
        };

        private static GameObject CreatePrimitive(slocGameObject obj, GameObject parent, PrimitiveObject primitive) {
            var toy = GameObject.CreatePrimitive(primitive.Type.ToPrimitiveType());
            toy.SetAbsoluteTransformFrom(parent);
            toy.SetLocalTransform(obj.Transform);
            var colliderMode = primitive.ColliderMode;
            if (colliderMode is not PrimitiveObject.ColliderCreationMode.Unset)
                toy.AddComponent<ColliderModeSetter>().mode = colliderMode;
            if (!TryGetMaterial(primitive.MaterialColor, out var mat, out var handle)) {
                if (handle)
                    HandleNoMaterial(primitive, toy);
                return toy;
            }

            toy.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return toy;
        }

        private static GameObject CreateLight(slocGameObject obj, GameObject parent, LightObject light) {
            var toy = new GameObject("Point Light");
            var lightComponent = toy.AddComponent<Light>();
            lightComponent.color = light.LightColor;
            lightComponent.intensity = light.Intensity;
            lightComponent.range = light.Range;
            lightComponent.shadows = light.Shadows ? LightShadows.Soft : LightShadows.None;
            toy.SetAbsoluteTransformFrom(parent);
            toy.SetLocalTransform(obj.Transform);
            return toy;
        }

        private static GameObject CreateEmpty(slocGameObject obj, GameObject parent) {
            var emptyObject = new GameObject("Empty");
            emptyObject.SetAbsoluteTransformFrom(parent);
            emptyObject.SetLocalTransform(obj.Transform);
            return emptyObject;
        }

        public static GameObject CreateObjects(IEnumerable<slocGameObject> objects, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) => CreateObjects(objects, out _, position, rotation, updateProgress);

        public static GameObject CreateObjects(IEnumerable<slocGameObject> objects, out int createdAmount, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) {
            var go = new GameObject {
                transform = {
                    position = position,
                    rotation = rotation
                }
            };
            var created = 0;
            var total = objects is ICollection<slocGameObject> l ? l.Count : -1;
            var processed = 0;
            var isCountKnown = total > 0;
            var floatTotal = (float) total;
            var createdInstances = new Dictionary<int, GameObject>();
            updateProgress?.Invoke("Creating objects", isCountKnown ? 0 : -1);
            foreach (var o in objects) {
                var gameObject = o.CreateObject(o.HasParent && createdInstances.TryGetValue(o.ParentId, out var parentInstance) ? parentInstance : go, false);
                if (gameObject != null) {
                    createdInstances[o.InstanceId] = gameObject;
                    created++;
                }

                processed++;
                updateProgress?.Invoke($"Creating objects ({processed}{(isCountKnown ? $" of {total}" : "")})", isCountKnown ? processed / floatTotal : -1);
            }

            createdAmount = created;
            return go;
        }

        public static GameObject CreateObjectsFromStream(Stream objects, out int spawnedAmount, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) => CreateObjects(ReadObjects(objects), out spawnedAmount, position, rotation, updateProgress);

        public static GameObject CreateObjectsFromFile(string path, out int spawnedAmount, Vector3 position, Quaternion rotation = default, Action<string, float> updateProgress = null) => CreateObjects(ReadObjectsFromFile(path, updateProgress), out spawnedAmount, position, rotation, updateProgress);

        #endregion

        #region Material Handling

        public static bool CreateForAll;
        public static bool SkipForAll;

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
            material = null;
            if (slocImporter.UseExistingMaterials)
                foreach (var e in AssetDatabase.FindAssets("t:material", slocImporter.SearchInColorsFolderOnly ? new[] {"Assets/Colors"} : null)) {
                    var asset = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(e));
                    if (!asset.mainTexture && asset.color == color) {
                        material = asset;
                        break;
                    }

                    EditorUtility.UnloadUnusedAssetsImmediate();
                }

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

        #endregion

        #region BinaryReader Extensions

        public static slocGameObject ReadObject(this BinaryReader stream, slocHeader header, ushort version = 0, IObjectReader objectReader = null) {
            objectReader ??= GetReader(version);
            return objectReader.Read(stream, header);
        }

        public static slocTransform ReadTransform(this BinaryReader reader) => new() {
            Position = reader.ReadVector(),
            Scale = reader.ReadVector(),
            Rotation = reader.ReadQuaternion()
        };

        public static Vector3 ReadVector(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Quaternion ReadQuaternion(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Color ReadColor(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Color ReadLossyColor(this BinaryReader reader) {
            var color = reader.ReadInt32();
            var red = color >> 24 & 0xFF;
            var green = color >> 16 & 0xFF;
            var blue = color >> 8 & 0xFF;
            var alpha = color & 0xFF;
            return new Color(red * ColorDivisionMultiplier, green * ColorDivisionMultiplier, blue * ColorDivisionMultiplier, alpha * ColorDivisionMultiplier);
        }

        public static int ReadObjectCount(this BinaryReader reader) {
            var count = reader.ReadInt32();
            return count < 0 ? 0 : count;
        }

        #endregion

        public static PrimitiveType ToPrimitiveType(this ObjectType type) => type switch {
            ObjectType.Cube => PrimitiveType.Cube,
            ObjectType.Sphere => PrimitiveType.Sphere,
            ObjectType.Capsule => PrimitiveType.Capsule,
            ObjectType.Cylinder => PrimitiveType.Cylinder,
            ObjectType.Plane => PrimitiveType.Plane,
            ObjectType.Quad => PrimitiveType.Quad,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "A non-primitive type was supplied")
        };

        public static void SetAbsoluteTransformFrom(this GameObject o, GameObject parent) {
            if (parent != null)
                o.transform.SetParent(parent.transform, false);
        }

        public static void SetLocalTransform(this GameObject o, slocTransform transform) {
            if (o == null)
                return;
            var t = o.transform;
            t.localPosition = transform.Position;
            t.localScale = transform.Scale;
            t.localRotation = transform.Rotation;
        }

        public static string AppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string ToFullAppDataPath(this string path) => path.Replace("%appdata%", AppData);

        public static string ToShortAppDataPath(this string path) => path.Replace('/', '\\').Replace(AppData, "%appdata%");

        public static IEnumerable<GameObject> WithAllChildren(this GameObject o) => o.GetComponentsInChildren<Transform>().Select(e => e.gameObject);

        public static bool HasFlagFast(this slocAttributes attributes, slocAttributes flag) => (attributes & flag) == flag;

        public static int ToRgbRange(this float f) => Mathf.FloorToInt(Mathf.Clamp01(f) * 255f);

        public static int ToLossyColor(this Color color) => color.r.ToRgbRange() << 24 | color.g.ToRgbRange() << 16 | color.b.ToRgbRange() << 8 | color.a.ToRgbRange();

        public static bool HasAttribute(this slocHeader header, slocAttributes attribute) => (header.Attributes & attribute) == attribute;

    }

}
