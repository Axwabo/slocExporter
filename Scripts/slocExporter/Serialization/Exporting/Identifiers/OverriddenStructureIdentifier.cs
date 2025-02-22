using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class OverriddenStructureIdentifier : IObjectIdentifier<StructureExportable>
    {

        public StructureExportable Process(GameObject o) => !o.TryGetComponent(out StructureOverride structureOverride)
            ? null
            : new StructureExportable {StructureType = structureOverride.type};

    }

}
