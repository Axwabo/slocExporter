using slocExporter.Serialization.Exporting.Exportables;
using TMPro;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class TextMeshProIdentifier : IObjectIdentifier<TextMeshProExportable>
    {

        public TextMeshProExportable Process(GameObject o)
            => !o.TryGetComponent(out TMP_Text text)
                ? null
                : new TextMeshProExportable(text);

    }

}
