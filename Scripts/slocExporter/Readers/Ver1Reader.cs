using System.IO;
using slocExporter.Objects;

namespace slocExporter.Readers {

    public class Ver1Reader : IObjectReader {

        public slocGameObject Read(BinaryReader stream) {
            var objectType = (ObjectType) stream.ReadByte();
            return objectType switch {
                ObjectType.Cube => ReadPrimitive(stream, objectType),
                ObjectType.Sphere => ReadPrimitive(stream, objectType),
                ObjectType.Cylinder => ReadPrimitive(stream, objectType),
                ObjectType.Plane => ReadPrimitive(stream, objectType),
                ObjectType.Capsule => ReadPrimitive(stream, objectType),
                ObjectType.Light => ReadLight(stream),
                _ => null
            };
        }

        private static slocGameObject ReadPrimitive(BinaryReader stream, ObjectType type) => new PrimitiveObject(0, type) {
            Transform = stream.ReadTransform(),
            MaterialColor = stream.ReadColor()
        };

        private static slocGameObject ReadLight(BinaryReader stream) => new LightObject(0) {
            Transform = stream.ReadTransform(),
            LightColor = stream.ReadColor(),
            Shadows = stream.ReadBoolean(),
            Range = stream.ReadSingle(),
            Intensity = stream.ReadSingle(),
        };

    }

}
