using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Processors
{

    public static class TextProcessor
    {

        public static void ProcessTransform(TextExportable text, RectTransform transform)
        {
            if (text.DisplaySize == Vector2.zero)
                text.DisplaySize = transform.rect.size;
        }

        public static void ProcessProperties(TextExportable text, TextProperties properties)
        {
            if (!string.IsNullOrEmpty(properties.format))
                text.Format = properties.format;
            text.Arguments = properties.arguments;
            if (properties.displaySize != Vector2.zero)
                text.DisplaySize = properties.displaySize;
        }

    }

}
