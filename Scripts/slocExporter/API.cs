using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using slocExporter.Objects;
using slocExporter.Readers;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using slocExporter.TriggerActions.Enums;
using UnityEngine;
using static slocExporter.MaterialHandler;

namespace slocExporter
{

    public static class API
    {

        public const float ColorDivisionMultiplier = 1f / 255f;

        public const ushort slocVersion = 5;

        public static string CurrentVersion = "4.0.1";

        #region Reader Declarations

        public static readonly IObjectReader DefaultReader = new Ver4Reader();

        private static readonly Dictionary<ushort, IObjectReader> VersionReaders = new()
        {
            {1, new Ver1Reader()},
            {2, new Ver2Reader()},
            {3, new Ver3Reader()},
            {4, new Ver4Reader()},
            {5, new Ver5Reader()}
        };

        public static bool TryGetReader(ushort version, out IObjectReader reader) => VersionReaders.TryGetValue(version, out reader);

        public static IObjectReader GetReader(ushort version) => TryGetReader(version, out var reader) ? reader : DefaultReader;

        #endregion

        #region Read

        private static readonly FieldInfo ReadPosField = typeof(BufferedStream).GetField("_readPos", BindingFlags.NonPublic | BindingFlags.Instance);

        // starting from v3, the version is only a ushort instead of a uint
        private static ushort ReadVersionSafe(BufferedStream buffered, BinaryReader binaryReader)
        {
            var newVersion = binaryReader.ReadUInt16();
            var oldVersion = binaryReader.ReadUInt16();
            if (oldVersion is 0)
                return (ushort) (newVersion | (uint) oldVersion << 16);
            var newPos = (int) ReadPosField.GetValue(buffered) - sizeof(ushort);
            ReadPosField.SetValue(buffered, newPos); // rewind the buffer by two bytes, so the whole stream won't be malformed data
            return newVersion;
        }

        public static List<slocGameObject> ReadObjects(Stream stream, bool autoClose = true, ProgressUpdater updateProgress = null)
        {
            var objects = new List<slocGameObject>();
            using var buffered = new BufferedStream(stream, 4);
            var binaryReader = new BinaryReader(buffered);
            var version = ReadVersionSafe(buffered, binaryReader);
            updateProgress?.Invoke("Reading objects", 0);
            var reader = GetReader(version);
            var header = reader.ReadHeader(binaryReader);
            var count = header.ObjectCount;
            var floatCount = (float) count;
            for (var i = 0; i < count; i++)
            {
                var obj = ReadObject(binaryReader, header, version, reader);
                if (obj is {IsValid: true})
                    objects.Add(obj);
                updateProgress?.Invoke($"Reading objects ({i + 1} of {count})", (i + 1) / floatCount);
            }

            if (autoClose)
                binaryReader.Close();
            return objects;
        }

        public static List<slocGameObject> ReadObjectsFromFile(string path, ProgressUpdater updateProgress = null) =>
            ReadObjects(File.OpenRead(path), true, updateProgress);

        #endregion

        #region Create

        public static GameObject CreateObject(this slocGameObject obj, GameObject parent = null, bool throwOnError = true) => obj switch
        {
            PrimitiveObject primitive => CreatePrimitive(parent, primitive),
            LightObject light => CreateLight(parent, light),
            EmptyObject => CreateEmpty(obj, parent),
            _ => throwOnError ? throw new ArgumentOutOfRangeException(nameof(obj.Type), obj.Type, "Unknown object type") : null
        };

        private static GameObject CreatePrimitive(GameObject parent, PrimitiveObject primitive)
        {
            var toy = GameObject.CreatePrimitive(primitive.Type.ToPrimitiveType());
            toy.SetAbsoluteTransformFrom(parent);
            toy.SetLocalTransform(primitive.Transform);
            var colliderMode = primitive.ColliderMode;
            if (colliderMode is not PrimitiveObject.ColliderCreationMode.Unset)
                toy.AddComponent<ColliderModeSetter>().mode = colliderMode;
            AddTriggerActionComponents(primitive.TriggerActions, toy);
            if (!TryGetMaterial(primitive.MaterialColor, out var mat, out var handle))
            {
                if (handle)
                    HandleNoMaterial(primitive, toy);
                return toy;
            }

            toy.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return toy;
        }

        private static void AddTriggerActionComponents(BaseTriggerActionData[] actions, GameObject gameObject)
        {
            foreach (var data in actions)
                if (data is SerializableTeleportToSpawnedObjectData tp)
                    TpToSpawnedCache.GetOrAdd(gameObject, () => new List<SerializableTeleportToSpawnedObjectData>()).Add(tp);
                else
                    gameObject.AddComponent<TriggerAction>().SetData(data);
        }

        private static GameObject CreateLight(GameObject parent, LightObject light)
        {
            var toy = new GameObject("Point Light");
            var lightComponent = toy.AddComponent<Light>();
            lightComponent.color = light.LightColor;
            lightComponent.intensity = light.Intensity;
            lightComponent.range = light.Range;
            lightComponent.shadows = light.Shadows ? LightShadows.Soft : LightShadows.None;
            toy.SetAbsoluteTransformFrom(parent);
            toy.SetLocalTransform(light.Transform);
            return toy;
        }

        private static GameObject CreateEmpty(slocGameObject obj, GameObject parent)
        {
            var emptyObject = new GameObject("Empty");
            emptyObject.SetAbsoluteTransformFrom(parent);
            emptyObject.SetLocalTransform(obj.Transform);
            return emptyObject;
        }

        public static GameObject CreateObjects(IEnumerable<slocGameObject> objects, Vector3 position, Quaternion rotation = default, ProgressUpdater updateProgress = null) => CreateObjects(objects, out _, position, rotation, updateProgress);

        private static readonly InstanceDictionary<GameObject> CreatedInstances = new();

        private static readonly Dictionary<GameObject, List<SerializableTeleportToSpawnedObjectData>> TpToSpawnedCache = new();

        public static GameObject CreateObjects(IEnumerable<slocGameObject> objects, out int createdAmount, Vector3 position, Quaternion rotation = default, ProgressUpdater updateProgress = null)
        {
            CreatedInstances.Clear();
            TpToSpawnedCache.Clear();
            try
            {
                var go = new GameObject
                {
                    transform =
                    {
                        position = position,
                        rotation = rotation
                    }
                };
                var created = 0;
                var total = objects is ICollection<slocGameObject> l ? l.Count : -1;
                var processed = 0;
                var isCountKnown = total > 0;
                var floatTotal = (float) total;
                ClearMaterialCache();
                updateProgress?.Invoke("Creating objects", isCountKnown ? 0 : -1);
                foreach (var o in objects)
                {
                    var gameObject = o.CreateObject(CreatedInstances.GetOrReturn(o.ParentId, go, o.HasParent), false);
                    if (gameObject != null)
                    {
                        CreatedInstances[o.InstanceId] = gameObject;
                        created++;
                    }

                    processed++;
                    updateProgress?.Invoke($"Creating objects ({processed}{(isCountKnown ? $" of {total}" : "")})", isCountKnown ? processed / floatTotal : -1);
                }

                PostProcessSpecialTriggerActions();
                createdAmount = created;
                return go;
            }
            finally
            {
                ClearMaterialCache();
                CreatedInstances.Clear();
                TpToSpawnedCache.Clear();
            }
        }

        private static void PostProcessSpecialTriggerActions()
        {
            foreach (var (o, list) in TpToSpawnedCache)
            foreach (var data in list)
            {
                if (!CreatedInstances.TryGetValue(data.ID, out var target))
                    continue;
                var component = o.AddComponent<TriggerAction>();
                component.type = TriggerActionType.TeleportToSpawnedObject;
                component.tpToSpawnedObject = new RuntimeTeleportToSpawnedObjectData(target, data.Offset)
                {
                    SelectedTargets = data.SelectedTargets,
                    Options = data.Options
                };
            }
        }

        public static GameObject CreateObjectsFromStream(Stream objects, out int spawnedAmount, Vector3 position, Quaternion rotation = default, ProgressUpdater updateProgress = null) => CreateObjects(ReadObjects(objects), out spawnedAmount, position, rotation, updateProgress);

        public static GameObject CreateObjectsFromFile(string path, out int spawnedAmount, Vector3 position, Quaternion rotation = default, ProgressUpdater updateProgress = null) => CreateObjects(ReadObjectsFromFile(path, updateProgress), out spawnedAmount, position, rotation, updateProgress);

        #endregion

        #region BinaryReader Extensions

        public static slocGameObject ReadObject(this BinaryReader stream, slocHeader header, ushort version = 0, IObjectReader objectReader = null)
        {
            objectReader ??= GetReader(version);
            return objectReader.Read(stream, header);
        }

        public static slocTransform ReadTransform(this BinaryReader reader) => new()
        {
            Position = reader.ReadVector(),
            Scale = reader.ReadVector(),
            Rotation = reader.ReadQuaternion()
        };

        public static Vector3 ReadVector(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Quaternion ReadQuaternion(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Color ReadColor(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static Color ReadLossyColor(this BinaryReader reader)
        {
            var color = reader.ReadInt32();
            var red = color >> 24 & 0xFF;
            var green = color >> 16 & 0xFF;
            var blue = color >> 8 & 0xFF;
            var alpha = color & 0xFF;
            return new Color(red * ColorDivisionMultiplier, green * ColorDivisionMultiplier, blue * ColorDivisionMultiplier, alpha * ColorDivisionMultiplier);
        }

        public static int ReadObjectCount(this BinaryReader reader)
        {
            var count = reader.ReadInt32();
            return count < 0 ? 0 : count;
        }

        public static float ReadShortAsFloat(this BinaryReader reader) => reader.ReadInt16() * TeleporterImmunityData.ShortToFloatMultiplier;

        #endregion

        #region BinaryWriter Extensions

        public static void WriteVector(this BinaryWriter writer, Vector3 vector3)
        {
            writer.Write(vector3.x);
            writer.Write(vector3.y);
            writer.Write(vector3.z);
        }

        public static void WriteQuaternion(this BinaryWriter writer, Quaternion quaternion)
        {
            writer.Write(quaternion.x);
            writer.Write(quaternion.y);
            writer.Write(quaternion.z);
            writer.Write(quaternion.w);
        }

        public static void WriteColor(this BinaryWriter writer, Color color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        public static void WriteFloatAsShort(this BinaryWriter writer, float value) => writer.Write((ushort) Mathf.Floor(value * TeleporterImmunityData.FloatToShortMultiplier));

        #endregion

        #region Bit Math

        public static PrimitiveObject.ColliderCreationMode CombineSafe(PrimitiveObject.ColliderCreationMode a, PrimitiveObject.ColliderCreationMode b) =>
            (PrimitiveObject.ColliderCreationMode) CombineSafe((byte) a, (byte) b);

        public static void SplitSafe(PrimitiveObject.ColliderCreationMode combined, out PrimitiveObject.ColliderCreationMode a, out PrimitiveObject.ColliderCreationMode b)
        {
            SplitSafe((byte) combined, out var x, out var y);
            a = (PrimitiveObject.ColliderCreationMode) x;
            b = (PrimitiveObject.ColliderCreationMode) y;
        }

        public static byte CombineSafe(byte a, byte b) => (byte) (a << 4 | b);

        public static void SplitSafe(byte combined, out byte a, out byte b)
        {
            a = (byte) (combined >> 4);
            b = (byte) (combined & 0xF);
        }

        #endregion

        public static PrimitiveType ToPrimitiveType(this ObjectType type) => type switch
        {
            ObjectType.Cube => PrimitiveType.Cube,
            ObjectType.Sphere => PrimitiveType.Sphere,
            ObjectType.Capsule => PrimitiveType.Capsule,
            ObjectType.Cylinder => PrimitiveType.Cylinder,
            ObjectType.Plane => PrimitiveType.Plane,
            ObjectType.Quad => PrimitiveType.Quad,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "A non-primitive type was supplied")
        };

        public static void SetAbsoluteTransformFrom(this GameObject o, GameObject parent)
        {
            if (parent != null)
                o.transform.SetParent(parent.transform, false);
        }

        public static void SetLocalTransform(this GameObject o, slocTransform transform)
        {
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

        public static bool IsTrigger(this PrimitiveObject.ColliderCreationMode colliderMode) => colliderMode is PrimitiveObject.ColliderCreationMode.Trigger or PrimitiveObject.ColliderCreationMode.NonSpawnedTrigger;

        public static bool HasAttribute(this slocHeader header, slocAttributes attribute) => (header.Attributes & attribute) == attribute;

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;
            value = factory();
            dictionary.Add(key, value);
            return value;
        }

    }

}
