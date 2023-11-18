using slocExporter.Objects;
using UnityEngine;

namespace slocExporter
{

    [DisallowMultipleComponent]
    public sealed class StructureOverride : MonoBehaviour
    {

        public StructureObject.StructureType type;

        public bool removeDefaultLoot;

    }

}
