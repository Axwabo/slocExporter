using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class InvisibleInteractableIdentifier : IObjectIdentifier<InvisibleInteractableExportable>
    {

        public InvisibleInteractableExportable Process(GameObject o)
            => !o.TryGetComponent(out InvisibleInteractableProperties properties)
                ? null
                : new InvisibleInteractableExportable
                {
                    Shape = !properties.overrideShape && TryIdentifyShape(o, out var shape) ? shape : properties.shape,
                    Locked = properties.locked
                };

        private static bool TryIdentifyShape(GameObject o, out InvisibleInteractableObject.ColliderShape shape)
        {
            if (!o.TryGetComponent(out Collider collider))
            {
                shape = InvisibleInteractableObject.ColliderShape.Box;
                return false;
            }

            shape = collider switch
            {
                BoxCollider box => Validate(box),
                CapsuleCollider capsule => Validate(capsule),
                SphereCollider sphere => Validate(sphere),
                _ => Unknown(collider)
            };
            return true;
        }

        private static InvisibleInteractableObject.ColliderShape Validate(BoxCollider box)
        {
            box.CheckGameSize();
            return InvisibleInteractableObject.ColliderShape.Box;
        }

        private static InvisibleInteractableObject.ColliderShape Validate(CapsuleCollider capsule)
        {
            capsule.CheckGameSize();
            return InvisibleInteractableObject.ColliderShape.Capsule;
        }

        private static InvisibleInteractableObject.ColliderShape Validate(SphereCollider sphere)
        {
            sphere.CheckGameSize();
            return InvisibleInteractableObject.ColliderShape.Sphere;
        }

        private static InvisibleInteractableObject.ColliderShape Unknown(Collider collider)
        {
            Debug.LogWarning($"Unknown collider {collider} found on this invisible interactable toy. Setting shape to Box.", collider);
            return InvisibleInteractableObject.ColliderShape.Box;
        }

    }

}
