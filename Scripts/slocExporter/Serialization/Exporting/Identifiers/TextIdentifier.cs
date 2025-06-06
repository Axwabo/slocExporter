using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class TextIdentifier : IObjectIdentifier<TextExportable>
    {

        public TextExportable Process(GameObject o)
            => !o.TryGetComponent(out TextProperties text)
                ? null
                : new TextExportable
                {
                    Format = text.format,
                    Arguments = text.arguments,
                    DisplaySize = text.displaySize
                };

    }

}
