using System.IO;
using UnityEngine;

namespace slocExporter.Objects {

    public sealed class slocTransform {

        public Vector3 Position = Vector3.zero;
        public Vector3 Scale = Vector3.one;
        public Quaternion Rotation = Quaternion.identity;

        public void WriteTo(BinaryWriter writer) {
            writer.WriteVector(Position);
            writer.WriteVector(Scale);
            writer.Write(Rotation.x);
            writer.Write(Rotation.y);
            writer.Write(Rotation.z);
            writer.Write(Rotation.w);
        }

        public static implicit operator slocTransform(Transform transform) => new() {
            Position = transform.localPosition,
            Scale = transform.localScale,
            Rotation = transform.localRotation
        };

    }

}
