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
            var resolved = PrimitiveObjectFlags.ServerCollider;
            var vetoed = PrimitiveObjectFlags.Visible;
            if (collider.isTrigger)
            {
                resolved |= PrimitiveObjectFlags.Trigger;
                vetoed |= PrimitiveObjectFlags.ClientCollider;
            }
            else
                resolved |= PrimitiveObjectFlags.ClientCollider;

            return new PrimitiveExportable
            {
                PrimitiveType = ObjectType.Cube,
                ResolvedFlags = resolved,
                VetoedFlags = vetoed
            };
        }

    }

}
