using System;
using slocExporter.Objects;
using TMPro;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public class TextExportable : IExportable<TextObject>
    {

        public string Format;

        public string[] Arguments;

        public Vector2 DisplaySize;

        public TextObject Export(int instanceId, ExportContext context) => new(Format, instanceId)
        {
            Arguments = Arguments,
            DisplaySize = DisplaySize
        };

    }

    public sealed class TextMeshProExportable : TextExportable
    {

        public TMP_Text Text { get; }

        public TextMeshProExportable(TMP_Text text)
        {
            Text = text;
            Arguments = Array.Empty<string>();
        }

    }

}
