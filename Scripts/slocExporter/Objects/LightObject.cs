using System.IO;
using slocExporter.Readers;
using UnityEngine;

namespace slocExporter.Objects {

    public sealed class LightObject : slocGameObject {

        public LightObject(int instanceId) : base(instanceId) => Type = ObjectType.Light;

        public Color LightColor = Color.white;

        public bool Shadows = true;

        public float Range = 5;

        public float Intensity = 1;

        protected override void WriteData(BinaryWriter writer, slocHeader header) {
            if (header.HasAttribute(slocAttributes.LossyColors))
                writer.Write(LightColor.ToLossyColor());
            else {
                writer.Write(LightColor.r);
                writer.Write(LightColor.g);
                writer.Write(LightColor.b);
                writer.Write(LightColor.a);
            }

            writer.Write(Shadows);
            writer.Write(Range);
            writer.Write(Intensity);
        }

    }

}
