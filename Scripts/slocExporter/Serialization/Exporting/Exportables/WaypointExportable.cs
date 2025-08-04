using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class WaypointExportable : IExportable<WaypointObject>
    {

        public bool VisualizeBounds;

        public bool IsStatic;

        public float Priority;

        public WaypointObject Export(int instanceId, ExportContext context) => new(instanceId)
        {
            Priority = Priority,
            IsStatic = IsStatic,
            VisualizeBounds = VisualizeBounds
        };

    }

}
