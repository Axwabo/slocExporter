using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class CameraExportable : IExportable<Scp079CameraObject>
    {

        public Scp079CameraType Type;

        public Scp079CameraProperties Properties;

        public Scp079CameraObject Export(int instanceId, ExportContext context) => new(Type, instanceId)
        {
            Label = Properties.label,
            CameraType = Type,
            VerticalMinimum = Properties.verticalMinimum,
            VerticalMaximum = Properties.verticalMaximum,
            HorizontalMinimum = Properties.horizontalMinimum,
            HorizontalMaximum = Properties.horizontalMaximum,
            ZoomMinimum = Properties.zoomMinimum,
            ZoomMaximum = Properties.zoomMaximum
        };

    }

}
