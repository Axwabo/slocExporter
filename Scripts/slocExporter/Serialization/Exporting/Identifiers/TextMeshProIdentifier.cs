using System;
using slocExporter.Serialization.Exporting.Exportables;
using TMPro;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class TextMeshProIdentifier : IObjectIdentifier<TextExportable>
    {

        public TextExportable Process(GameObject o)
        {
            if (!o.TryGetComponent(out TMP_Text text))
                return null;
            if (!Mathf.Approximately(text.fontSize, 1))
                Debug.LogWarning("Inconsistent font size. Set it to 1 to ensure parity with the SL client.", text);
            return new TextExportable
            {
                Format = text.text,
                Arguments = Array.Empty<string>()
            };
        }

    }

}
