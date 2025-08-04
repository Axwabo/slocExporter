using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class StructureExportable : IExportable<StructureObject>
    {

        public StructureObject.StructureType StructureType;

        public StructureObject Export(int instanceId, ExportContext context) => new(instanceId, StructureType);

    }

}
