using slocExporter.Objects;
using UnityEngine;

namespace slocExporter
{

    public sealed class InvisibleInteractableProperties : MonoBehaviour
    {

        [Min(0)]
        [Tooltip("Search duration in seconds. The OnInteracted event is called if the search duration is 0, otherwise, the search events are invoked.")]
        public float interactionDuration;

        [Tooltip("If false, the shape will be based on whether the collider is a BoxCollider, SphereCollider or CapsuleCollider.")]
        public bool overrideShape;

        [Tooltip("Used if overrideShape is false, or if no collider is present.")]
        public InvisibleInteractableObject.ColliderShape shape;

        [Tooltip("Set to true to disable searching.")]
        public bool locked;

    }

}
