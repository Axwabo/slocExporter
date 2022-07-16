using System.IO;
using slocExporter.Objects;

namespace slocExporter.Readers {

    public class Ver1Reader : IObjectReader {

        public slocGameObject Read(BinaryReader stream) {
            var type = (ObjectType) stream.ReadByte();
            return type switch {
                ObjectType.Cube => ReadPrimitive(stream, type),
                ObjectType.Sphere => ReadPrimitive(stream, type),
                ObjectType.Cylinder => ReadPrimitive(stream, type),
                ObjectType.Plane => ReadPrimitive(stream, type),
                ObjectType.Capsule => ReadPrimitive(stream, type),
                ObjectType.Light => ReadLight(stream),
                _ => null
            };
        }

        private static slocGameObject ReadPrimitive(BinaryReader stream, ObjectType type) {
            var obj = new PrimitiveObject(type) {
                Transform = stream.ReadTransform(),
                MaterialColor = stream.ReadColor()
            };
            return obj;
        }

        private static slocGameObject ReadLight(BinaryReader stream) {
            var obj = new LightObject {
                Transform = stream.ReadTransform(),
                LightColor = stream.ReadColor(),
                Shadows = stream.ReadBoolean(),
                Range = stream.ReadSingle(),
                Intensity = stream.ReadSingle(),
            };
            return obj;
        }

    }

}
