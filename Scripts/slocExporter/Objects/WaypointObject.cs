using System.IO;
using slocExporter.Extensions;
using slocExporter.Readers;

namespace slocExporter.Objects
{

    public sealed class WaypointObject : slocGameObject
    {

        public WaypointObject(int instanceId = 0)
            : base(instanceId)
            => Type = ObjectType.Waypoint;

        public float Priority;

        public bool IsStatic;

        public bool VisualizeBounds;

        protected override void WriteData(BinaryWriter writer, slocHeader header)
        {
            writer.Write(Priority);
            writer.WriteTwoBools(IsStatic, VisualizeBounds);
        }

    }

}
