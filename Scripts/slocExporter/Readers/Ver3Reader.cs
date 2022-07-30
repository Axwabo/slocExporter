using System.IO;
using slocExporter.Objects;

namespace slocExporter.Readers {

    public class Ver3Reader : IObjectReader {

        public slocHeader ReadHeader(BinaryReader stream) => new(stream.ReadInt32(), stream.ReadByte());

        public slocGameObject Read(BinaryReader stream, slocAttributes attributes) {
            var objectType = (ObjectType) stream.ReadByte();
            return objectType switch {
                ObjectType.Cube => ReadPrimitive(stream, objectType, attributes),
                ObjectType.Sphere => ReadPrimitive(stream, objectType, attributes),
                ObjectType.Cylinder => ReadPrimitive(stream, objectType, attributes),
                ObjectType.Plane => ReadPrimitive(stream, objectType, attributes),
                ObjectType.Capsule => ReadPrimitive(stream, objectType, attributes),
                ObjectType.Light => ReadLight(stream, attributes),
                ObjectType.Empty => ReadEmpty(stream),
                _ => null
            };
        }

        public static slocGameObject ReadPrimitive(BinaryReader stream, ObjectType type, slocAttributes attributes) => new PrimitiveObject(stream.ReadInt32(), type) {
            ParentId = stream.ReadInt32(),
            Transform = stream.ReadTransform(),
            MaterialColor = attributes.HasFlagFast(slocAttributes.LossyColors) ? stream.ReadLossyColor() : stream.ReadColor()
        };

        public static slocGameObject ReadLight(BinaryReader stream, slocAttributes attributes) => new LightObject(stream.ReadInt32()) {
            ParentId = stream.ReadInt32(),
            Transform = stream.ReadTransform(),
            LightColor = attributes.HasFlagFast(slocAttributes.LossyColors) ? stream.ReadLossyColor() : stream.ReadColor(),
            Shadows = stream.ReadBoolean(),
            Range = stream.ReadSingle(),
            Intensity = stream.ReadSingle(),
        };

        public static slocGameObject ReadEmpty(BinaryReader stream) => new EmptyObject(stream.ReadInt32()) {
            ParentId = stream.ReadInt32(),
            Transform = stream.ReadTransform()
        };

    }

}
