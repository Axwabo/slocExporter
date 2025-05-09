using System.Linq;
using slocExporter.Extensions;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEditor;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class CapybaraIdentifier : IObjectIdentifier<CapybaraExportable>
    {

        public static readonly GUID CapybaraGuid = new("e5e88ac8d3898404997ae39ca057dcf9");

        public CapybaraExportable Process(GameObject o)
            => o.TryGetPrefabGuid(out var guid) && guid == CapybaraGuid
                ? new CapybaraExportable {Collidable = o.GetComponentsInChildren<Collider>(true).All(e => e.enabled)}
                : null;

    }

}
