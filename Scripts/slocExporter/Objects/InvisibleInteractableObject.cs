using System.IO;
using slocExporter.Readers;

namespace slocExporter.Objects
{

    public sealed class InvisibleInteractableObject : slocGameObject
    {

        public InvisibleInteractableObject(ColliderShape shape, int instanceId = 0)
            : base(instanceId)
            => Shape = shape;

        public ColliderShape Shape;

        public float InteractionDuration;

        public override bool IsValid => true;

        protected override void WriteData(BinaryWriter writer, slocHeader header)
        {
            writer.Write((byte) Shape);
            writer.Write(InteractionDuration);
        }

        public enum ColliderShape
        {

            Box,
            Sphere,
            Capsule

        }

    }

}
