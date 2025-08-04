using UnityEngine;

namespace slocExporter
{

    public sealed class CullingParent : MonoBehaviour
    {

        public Vector3 size;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, size);
        }

    }

}
