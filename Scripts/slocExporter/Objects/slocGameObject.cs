using System.IO;
using slocExporter.Extensions;
using slocExporter.Readers;

namespace slocExporter.Objects
{

    public abstract class slocGameObject
    {

        protected slocGameObject(int instanceId) => InstanceId = instanceId;

        public readonly int InstanceId;

        public int ParentId = 0;

        public bool HasParent => ParentId != InstanceId;

        public ObjectType Type { get; protected set; } = ObjectType.None;

        public slocTransform Transform = new();

        public string Name;

        public string Tag;

        public virtual bool IsValid => Type != ObjectType.None;

        public void WriteTo(BinaryWriter writer, slocHeader header)
        {
            writer.Write((byte) Type);
            writer.Write(InstanceId);
            writer.Write(ParentId);
            Transform.WriteTo(writer);
            if (header.HasAttribute(slocAttributes.NamesAndTags))
            {
                writer.WriteNullableString(Name);
                writer.WriteNullableString(Tag);
            }

            WriteData(writer, header);
        }

        protected virtual void WriteData(BinaryWriter writer, slocHeader header)
        {
        }

    }

}
