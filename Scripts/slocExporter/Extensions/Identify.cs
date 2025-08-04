using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Extensions
{

    public static class Identify
    {

        public const string ExporterIgnoredTag = "slocExporter Ignored";
        public const string RoomTag = "Room";

        public static readonly Dictionary<Regex, ObjectType> PrimitiveTypes = new Dictionary<string, ObjectType>
        {
            {"Cube", ObjectType.Cube},
            {"Cylinder", ObjectType.Cylinder},
            {"Sphere", ObjectType.Sphere},
            {"Capsule", ObjectType.Capsule},
            {"Plane", ObjectType.Plane},
            {"Quad", ObjectType.Quad}
        }.ToDictionary(k => new Regex($"{k.Key}(?:(?: Instance)+)?"), v => v.Value);

        [Obsolete("Prefab GUIDs are unreliable", error: true)]
        public static readonly Dictionary<string, StructureObject.StructureType> StructureGuids
            = API.StructureGuids.ToDictionary(k => k.Value, v => v.Key);

        [Obsolete("Prefab GUIDs are unreliable", error: true)]
        public static readonly Dictionary<string, Scp079CameraType> CameraGuids
            = API.CameraGuids.ToDictionary(k => k.Value, v => v.Key);

        public static readonly Dictionary<string, StructureObject.StructureType> StructurePrefabNames
            = API.StructurePrefabNames.ToDictionary(k => k.Value, v => v.Key);

        public static readonly Dictionary<string, Scp079CameraType> CameraPrefabNames
            = API.CameraPrefabNames.ToDictionary(k => k.Value, v => v.Key);

        public static ObjectType PrimitiveObjectType(string meshName) => PrimitiveTypes.FirstOrDefault(e => e.Key.IsMatch(meshName)).Value;

        public static bool IsIgnored(this GameObject o) => o.tag is ExporterIgnoredTag or RoomTag || o.TryGetComponent(out ExporterIgnored _);

    }

}
