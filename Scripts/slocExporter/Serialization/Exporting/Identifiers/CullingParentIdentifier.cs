using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class CullingParentIdentifier : IObjectIdentifier<CullingParentExportable>
    {

        public CullingParentExportable Process(GameObject o)
            => !o.TryGetComponent(out CullingParent parent)
                ? null
                : new CullingParentExportable {Size = parent.size};

    }

}
