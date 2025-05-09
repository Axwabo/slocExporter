using System.IO;
using slocExporter.Extensions;
using slocExporter.Readers;

namespace slocExporter.Objects
{

    public sealed class Scp079CameraObject : slocGameObject
    {

        public Scp079CameraObject(Scp079CameraType cameraType, int instanceId = 0) : base(instanceId)
        {
            CameraType = cameraType;
            Type = ObjectType.Scp079Camera;
        }

        public override bool IsValid => CameraType != Scp079CameraType.None;

        public string Label;

        public Scp079CameraType CameraType;

        public float VerticalMinimum;

        public float VerticalMaximum;

        public float HorizontalMinimum;

        public float HorizontalMaximum;

        public float ZoomMinimum;

        public float ZoomMaximum;

        protected override void WriteData(BinaryWriter writer, slocHeader header)
        {
            writer.WriteNullableString(Label);
            writer.Write((byte) CameraType);
            writer.Write(VerticalMinimum);
            writer.Write(VerticalMaximum);
            writer.Write(HorizontalMinimum);
            writer.Write(HorizontalMaximum);
            writer.Write(ZoomMinimum);
            writer.Write(ZoomMaximum);
        }

    }

}
