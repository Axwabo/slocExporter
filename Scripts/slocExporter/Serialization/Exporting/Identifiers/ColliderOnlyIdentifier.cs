using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class ColliderOnlyIdentifier : IObjectIdentifier<PrimitiveExportable>
    {

        public PrimitiveExportable Process(GameObject o)
        {
            var components = o.GetComponents<Component>();
            if (components.Length != 2 || components[0] is not Transform || components[1] is not BoxCollider collider)
                return null;
            collider.CheckGameSize();
            return new PrimitiveExportable
            {
                PrimitiveType = ObjectType.Cube,
                VetoedFlags = PrimitiveObjectFlags.Visible
            };
        }

    }

}
