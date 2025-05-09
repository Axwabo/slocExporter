using slocExporter.Extensions;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class PrefabStructureIdentifier : IObjectIdentifier<StructureExportable>
    {

        public static readonly PrefabStructureIdentifier Instance = new();

        public StructureExportable Process(GameObject o)
            => o.TryGetPrefabGuid(out var guid) && Identify.StructureGuids.TryGetValue(guid.ToString(), out var type)
                ? new StructureExportable {StructureType = type}
                : null;

    }

}
