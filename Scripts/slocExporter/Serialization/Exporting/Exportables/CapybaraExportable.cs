using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class CapybaraExportable : IExportable<CapybaraObject>
    {

        public bool Collidable;

        public CapybaraObject Export(int instanceId, ExportContext context) => new(instanceId) {Collidable = Collidable};

    }

}
