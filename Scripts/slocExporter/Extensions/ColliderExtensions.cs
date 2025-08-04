using UnityEngine;

namespace slocExporter.Extensions
{

    public static class ColliderExtensions
    {

        private const string Suffix = " for its scale to apply properly in SL. Modify the transform to shape the collider.";

        private const string Box = "The BoxCollider's center must be (0, 0, 0) and its size must be (1, 1, 1)" + Suffix;
        private const string Capsule = "The CapsuleCollider must be Y-aligned, its center must be (0, 0, 0), its height must be 2 and its radius must be 0.5" + Suffix;
        private const string Sphere = "The SphereCollider's center must be (0, 0, 0) and its radius must be 0.5" + Suffix;

        public static void CheckGameSize(this BoxCollider collider)
        {
            if (collider.center != Vector3.zero || collider.size != Vector3.one)
                Debug.LogWarning(Box, collider);
        }

        public static void CheckGameSize(this CapsuleCollider collider)
        {
            if (collider.center != Vector3.zero || !Mathf.Approximately(collider.height, 2) || !Mathf.Approximately(collider.radius, 0.5f) || collider.direction != 1)
                Debug.LogWarning(Capsule, collider);
        }

        public static void CheckGameSize(this SphereCollider collider)
        {
            if (collider.center != Vector3.zero || !Mathf.Approximately(collider.radius, 0.5f))
                Debug.LogWarning(Sphere, collider);
        }

    }

}
