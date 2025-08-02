using UnityEngine;

namespace slocExporter
{

    public sealed class Waypoint : MonoBehaviour
    {

        private static readonly Vector3 Size = Vector3.one * 256;

        [Tooltip("When two waypoints overlap, the one with higher squared priority will be used.")]
        public float priority;

        [Tooltip("Should only be enabled if used for extending the map area.")]
        public bool isStatic;

        [Tooltip("Whether the bounds should be visible in-game.")]
        public bool visualizeBounds;

        private void OnDrawGizmos()
        {
            if (visualizeBounds)
                OnDrawGizmosSelected();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = isStatic ? Matrix4x4.Translate(transform.position) : transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Size);
        }

    }

}
