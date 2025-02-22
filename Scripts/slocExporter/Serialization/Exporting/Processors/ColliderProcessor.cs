using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Processors
{

    public static class ColliderProcessor
    {

        public static void Process(PrimitiveExportable primitive, Collider collider)
        {
            if (!collider.enabled)
            {
                primitive.VetoedFlags |= PrimitiveObjectFlags.ClientCollider | PrimitiveObjectFlags.ServerCollider | PrimitiveObjectFlags.Trigger;
                return;
            }

            if (!collider.isTrigger)
            {
                primitive.ResolvedFlags |= PrimitiveObjectFlags.ClientCollider | PrimitiveObjectFlags.ServerCollider;
                return;
            }

            primitive.ResolvedFlags |= PrimitiveObjectFlags.ServerCollider | PrimitiveObjectFlags.Trigger;
            primitive.VetoedFlags |= PrimitiveObjectFlags.ClientCollider;
        }

    }

}
