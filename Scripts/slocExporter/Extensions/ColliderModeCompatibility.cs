using System;
using slocExporter.Objects;
using static slocExporter.Objects.PrimitiveObject;

#pragma warning disable CS0618 // Type or member is obsolete

namespace slocExporter.Extensions
{

    public static class ColliderModeCompatibility
    {

        public static PrimitiveObjectFlags GetPrimitiveFlags(ColliderCreationMode mode) => PrimitiveObjectFlags.Visible | mode switch
        {
            ColliderCreationMode.Unset => PrimitiveObjectFlags.None,
            ColliderCreationMode.NoCollider => PrimitiveObjectFlags.None,
            ColliderCreationMode.ClientOnly => PrimitiveObjectFlags.ClientCollider,
            ColliderCreationMode.ServerOnly => PrimitiveObjectFlags.ServerCollider,
            ColliderCreationMode.Both => PrimitiveObjectFlags.ClientCollider | PrimitiveObjectFlags.ServerCollider,
            ColliderCreationMode.Trigger => PrimitiveObjectFlags.ServerCollider | PrimitiveObjectFlags.Trigger,
            ColliderCreationMode.NonSpawnedTrigger => PrimitiveObjectFlags.ServerCollider | PrimitiveObjectFlags.Trigger | PrimitiveObjectFlags.NotSpawned,
            ColliderCreationMode.ServerOnlyNonSpawned => PrimitiveObjectFlags.ServerCollider | PrimitiveObjectFlags.NotSpawned,
            ColliderCreationMode.NoColliderNonSpawned => PrimitiveObjectFlags.NotSpawned,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown collider creation mode")
        };

    }

}
