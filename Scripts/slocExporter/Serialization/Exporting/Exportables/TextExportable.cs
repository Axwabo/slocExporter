using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class TextExportable : IExportable<TextObject>
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

}
