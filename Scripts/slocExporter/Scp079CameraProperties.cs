using slocExporter.Objects;
using UnityEngine;

namespace slocExporter
{

    public sealed class Scp079CameraProperties : MonoBehaviour
    {

        [Tooltip("If None, the prefab ID will be used to identify the camera.")]
        public Scp079CameraType typeOverride;

        public string label;

        [Tooltip("Minimum relative vertical rotation in degrees")]
        public float verticalMinimum;

        [Tooltip("Maximum relative vertical rotation in degrees")]
        public float verticalMaximum;

        [Tooltip("Minimum relative horizontal rotation in degrees")]
        public float horizontalMinimum;

        [Tooltip("Maximum relative horizontal rotation in degrees")]
        public float horizontalMaximum;

        [Range(0, 1)]
        [Tooltip("Percentage of minimum zoom")]
        public float zoomMinimum;

        [Range(0, 1)]
        [Tooltip("Percentage of maximum zoom")]
        public float zoomMaximum;

        private void OnDrawGizmosSelected()
        {
            var t = transform;
            var position = t.position;
            var rotation = t.rotation;
            DrawGizmo(position, rotation, verticalMinimum, 0, Color.red);
            DrawGizmo(position, rotation, verticalMaximum, 0, Color.blue);
            DrawGizmo(position, rotation, 0, horizontalMinimum, Color.green);
            DrawGizmo(position, rotation, 0, horizontalMaximum, Color.yellow);
        }

        private static void DrawGizmo(Vector3 position, Quaternion rotation, float x, float y, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(position, rotation * Quaternion.Euler(x, y, 0) * Vector3.forward);
        }

    }

}
