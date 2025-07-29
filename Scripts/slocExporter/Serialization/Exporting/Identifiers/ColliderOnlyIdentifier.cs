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
            if (collider.center != Vector3.zero || collider.size != Vector3.one)
                Debug.LogWarning("The BoxCollider's center must be (0, 0, 0) and its size must be (1, 1, 1) for its scale to apply properly in SL.", collider);
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
