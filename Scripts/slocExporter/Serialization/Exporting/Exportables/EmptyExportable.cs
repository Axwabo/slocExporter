using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class EmptyExportable : IExportable<EmptyObject>
    {

        public static readonly EmptyExportable Instance = new();

        public EmptyObject Export(int instanceId, ExportContext context) => new(instanceId);

    }

}
