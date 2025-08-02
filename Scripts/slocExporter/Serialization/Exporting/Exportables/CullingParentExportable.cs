using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class CullingParentExportable : IExportable<CullingParentObject>
    {

        public Vector3 Size;

        public CullingParentObject Export(int instanceId, ExportContext context) => new(instanceId) {BoundsSize = Size};

    }

}
