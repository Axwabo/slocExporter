using System.IO;
using slocExporter.Readers;

namespace slocExporter.Objects
{

    public sealed class CapybaraObject : slocGameObject
    {

        public CapybaraObject() : this(0)
        {
        }

        public CapybaraObject(int instanceId) : base(instanceId) => Type = ObjectType.Capybara;

        public bool Collidable;

        protected override void WriteData(BinaryWriter writer, slocHeader header) => writer.Write(Collidable);

    }

}
