using System.IO;
using slocExporter.Extensions;
using slocExporter.Readers;

namespace slocExporter.Objects
{

    public sealed class InvisibleInteractableObject : slocGameObject
    {

        public InvisibleInteractableObject(int instanceId = 0)
            : base(instanceId)
            => Type = ObjectType.InvisibleInteractable;

        public ColliderShape Shape;

        public bool Locked;

        public float InteractionDuration;

        public override bool IsValid => true;

        protected override void WriteData(BinaryWriter writer, slocHeader header)
        {
            writer.WriteByteWithBool((byte) Shape, Locked);
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
