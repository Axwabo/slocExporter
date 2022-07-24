﻿using System.IO;
using slocExporter.Objects;

namespace slocExporter.Readers {

    public class Ver1Reader : IObjectReader {

        public slocGameObject Read(BinaryReader stream) => (ObjectType) stream.ReadByte() switch {
            ObjectType.Cube => ReadPrimitive(stream, (ObjectType) stream.ReadByte()),
            ObjectType.Sphere => ReadPrimitive(stream, (ObjectType) stream.ReadByte()),
            ObjectType.Cylinder => ReadPrimitive(stream, (ObjectType) stream.ReadByte()),
            ObjectType.Plane => ReadPrimitive(stream, (ObjectType) stream.ReadByte()),
            ObjectType.Capsule => ReadPrimitive(stream, (ObjectType) stream.ReadByte()),
            ObjectType.Light => ReadLight(stream),
            _ => null
        };

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