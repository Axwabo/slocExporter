using System;
using System.Linq;
using slocExporter.Extensions;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEditor;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class CapybaraIdentifier : IObjectIdentifier<CapybaraExportable>
    {

        [Obsolete("Prefab GUIDs are unreliable")]
        public static readonly GUID CapybaraGuid = new("e5e88ac8d3898404997ae39ca057dcf9");

        public const string CapybaraPrefabName = "CapybaraToy";

        public CapybaraExportable Process(GameObject o)
            => o.TryGetPrefabName(out var name) && name == CapybaraPrefabName
                ? new CapybaraExportable {Collidable = o.GetComponentsInChildren<Collider>(true).All(e => e.enabled)}
                : null;

    }

}
