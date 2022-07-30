using System;
using System.IO;
using UnityEngine;

namespace slocExporter.Objects {

    public class PrimitiveObject : slocGameObject {

        public PrimitiveObject(int instanceId, ObjectType type) : base(instanceId) {
            if (type is ObjectType.None or ObjectType.Light)
                throw new ArgumentException("Invalid primitive type", nameof(type));
            Type = type;
        }

        public Color MaterialColor;

        public override void WriteTo(BinaryWriter writer, slocAttributes attributes) {
            base.WriteTo(writer, attributes);
            if (attributes.HasFlagFast(slocAttributes.LossyColors)) {
                writer.Write(MaterialColor.ToLossyColor());
                return;
            }

            writer.Write(MaterialColor.r);
            writer.Write(MaterialColor.g);
            writer.Write(MaterialColor.b);
            writer.Write(MaterialColor.a);
        }

    }

}
