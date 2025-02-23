using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class EmptyIdentifier : IObjectIdentifier<EmptyExportable>
    {

        public EmptyExportable Process(GameObject o)
        {
            var components = o.GetComponents<Component>();
            return components.Length == 1 && components[0] is Transform
                ? EmptyExportable.Instance
                : null;
        }

    }

}
