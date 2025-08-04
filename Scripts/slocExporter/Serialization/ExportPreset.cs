using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Serialization
{

    [CreateAssetMenu(fileName = DefaultName, menuName = "sloc/Export Preset", order = 0)]
    public sealed class ExportPreset : ScriptableObject
    {

        public const string DefaultName = "sloc Export Preset";

        [Tooltip("Uses a single 32-bit integer for colors instead of four 32-bit floats (16 bytes per color). This reduces file size but limits the RGB color range to 0-255 and therefore loses precision.")]
        public bool lossyColors;

        [Tooltip("The default flags to use for primitive objects.")]
        public PrimitiveObjectFlags defaultPrimitiveFlags;

        [Tooltip("Exports trigger actions for every primitive, even if their flags don't specify it as a trigger.")]
        public bool exportAllTriggerActions;

        [Tooltip("Includes the name and tag (if any) of each object.")]
        public bool exportNamesAndTags;

        public bool includeInactiveObjects;

        [Tooltip("Whether to include prefabs' children. Outermost prefab root objects are always exported.")]
        public bool traversePrefabs;

    }

}
