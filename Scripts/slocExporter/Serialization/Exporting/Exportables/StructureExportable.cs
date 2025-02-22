using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class StructureExportable : IExportable<StructureObject>
    {

        public StructureObject.StructureType StructureType;

        public StructureObject Export(int instanceId) => new(instanceId, StructureType);

    }

}
