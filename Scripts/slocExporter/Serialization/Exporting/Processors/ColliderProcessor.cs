using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Processors
{

    public static class ColliderProcessor
    {

        public static void Process(PrimitiveExportable primitive, Collider collider)
        {
            if (collider.isTrigger)
                primitive.ResolvedFlags |= PrimitiveObjectFlags.Trigger;

            if (!collider.enabled)
            {
                primitive.VetoedFlags |= PrimitiveObjectFlags.ClientCollider | PrimitiveObjectFlags.ServerCollider;
                return;
            }

            if (!collider.isTrigger)
            {
                primitive.ResolvedFlags |= PrimitiveObjectFlags.ClientCollider | PrimitiveObjectFlags.ServerCollider;
                return;
            }

            primitive.ResolvedFlags |= PrimitiveObjectFlags.ServerCollider;
            primitive.VetoedFlags |= PrimitiveObjectFlags.ClientCollider;
        }

    }

}
