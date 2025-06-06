using UnityEngine;

namespace slocExporter
{

    public sealed class TextProperties : MonoBehaviour
    {

        [Multiline]
        [Tooltip("The string.Format-compatible text. If empty, the TMPro text will be used")]
        public string format;

        [Multiline]
        public string[] arguments;

        [Tooltip("If (0,0), the RectTransform width & height will be used")]
        public Vector2 displaySize;

    }

}
