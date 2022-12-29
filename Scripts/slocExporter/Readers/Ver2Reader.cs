﻿using System.IO;
using slocExporter.Objects;

namespace slocExporter.Readers {

    public sealed class Ver2Reader : IObjectReader {

        public slocHeader ReadHeader(BinaryReader stream) => new(stream.ReadObjectCount());

        public slocGameObject Read(BinaryReader stream, slocHeader header) {
            var objectType = (ObjectType) stream.ReadByte();
            return objectType switch {
                ObjectType.Cube => ReadPrimitive(stream, objectType),
                ObjectType.Sphere => ReadPrimitive(stream, objectType),
                ObjectType.Cylinder => ReadPrimitive(stream, objectType),
                ObjectType.Plane => ReadPrimitive(stream, objectType),
                ObjectType.Capsule => ReadPrimitive(stream, objectType),
                ObjectType.Light => ReadLight(stream),
                ObjectType.Empty => ReadEmpty(stream),
                _ => null
            };
        }

        public static slocGameObject ReadPrimitive(BinaryReader stream, ObjectType type) {
            var instanceId = stream.ReadInt32();
            var parentId = stream.ReadInt32();
            var transform = stream.ReadTransform();
            var materialColor = stream.ReadColor();
            return new PrimitiveObject(instanceId, type) {
                ParentId = parentId,
                Transform = transform,
                MaterialColor = materialColor
            };
        }

        public static slocGameObject ReadLight(BinaryReader stream) {
            var instanceId = stream.ReadInt32();
            var parentId = stream.ReadInt32();
            var transform = stream.ReadTransform();
            var lightColor = stream.ReadColor();
            var shadows = stream.ReadBoolean();
            var range = stream.ReadSingle();
            var intensity = stream.ReadSingle();
            return new LightObject(instanceId) {
                ParentId = parentId,
                Transform = transform,
                LightColor = lightColor,
                Shadows = shadows,
                Range = range,
                Intensity = intensity,
            };
        }

        public static slocGameObject ReadEmpty(BinaryReader stream) {
            var instanceId = stream.ReadInt32();
            var parentId = stream.ReadInt32();
            var transform = stream.ReadTransform();
            return new EmptyObject(instanceId) {
                ParentId = parentId,
                Transform = transform
            };
        }

    }

}
