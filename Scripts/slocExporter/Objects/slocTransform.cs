using System.IO;
using UnityEngine;

namespace slocExporter.Objects {

    public class slocTransform {

        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public void WriteTo(BinaryWriter writer) {
            writer.Write(Position.x);
            writer.Write(Position.y);
            writer.Write(Position.z);
            writer.Write(Scale.x);
            writer.Write(Scale.y);
            writer.Write(Scale.z);
            writer.Write(Rotation.x);
            writer.Write(Rotation.y);
            writer.Write(Rotation.z);
            writer.Write(Rotation.w);
        }

    }

}
