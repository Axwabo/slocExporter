using System.IO;

namespace slocExporter.Objects {

    public abstract class slocGameObject {

        public ObjectType Type { get; protected set; } = ObjectType.None;
        public slocTransform Transform = new slocTransform();

        public override string ToString() => $"{Type};{Transform}";

        public virtual bool IsEmpty => Type == ObjectType.None;

        public virtual void WriteTo(BinaryWriter writer) {
            writer.Write((byte) Type);
            Transform.WriteTo(writer);
        }

    }

}
