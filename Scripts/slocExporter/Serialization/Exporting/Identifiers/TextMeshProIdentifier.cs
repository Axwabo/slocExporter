using System;
using slocExporter.Serialization.Exporting.Exportables;
using TMPro;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class TextMeshProIdentifier : IObjectIdentifier<TextExportable>
    {

        public TextExportable Process(GameObject o)
            => !o.TryGetComponent(out TMP_Text text)
                ? null
                : new TextExportable
                {
                    Format = text.text,
                    Arguments = Array.Empty<string>()
                };

    }

}
