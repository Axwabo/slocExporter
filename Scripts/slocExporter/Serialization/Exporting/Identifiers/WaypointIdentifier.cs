using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class WaypointIdentifier : IObjectIdentifier<WaypointExportable>
    {

        public const string Warning = "Static waypoints should have a world-space scale of (1, 1, 1).";

        public WaypointExportable Process(GameObject o)
            => !o.TryGetComponent(out Waypoint waypoint)
                ? null
                : new WaypointExportable
                {
                    Priority = waypoint.priority,
                    IsStatic = waypoint.isStatic,
                    VisualizeBounds = waypoint.visualizeBounds
                };

    }

}
