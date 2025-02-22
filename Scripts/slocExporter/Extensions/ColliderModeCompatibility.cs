using System;
using slocExporter.Objects;

#pragma warning disable CS0618 // Type or member is obsolete

namespace slocExporter.Extensions
{

    public static class ColliderModeCompatibility
    {

        public static PrimitiveObjectFlags GetCollisionFlags(PrimitiveObject.ColliderCreationMode mode) => mode switch
        {
            PrimitiveObject.ColliderCreationMode.Unset => PrimitiveObjectFlags.None,
            PrimitiveObject.ColliderCreationMode.NoCollider => PrimitiveObjectFlags.None,
            PrimitiveObject.ColliderCreationMode.ClientOnly => PrimitiveObjectFlags.ClientCollidable,
            PrimitiveObject.ColliderCreationMode.ServerOnly => PrimitiveObjectFlags.ServerCollidable,
            PrimitiveObject.ColliderCreationMode.Both => PrimitiveObjectFlags.ClientCollidable | PrimitiveObjectFlags.ServerCollidable,
            PrimitiveObject.ColliderCreationMode.Trigger => PrimitiveObjectFlags.Trigger,
            PrimitiveObject.ColliderCreationMode.NonSpawnedTrigger => PrimitiveObjectFlags.Trigger | PrimitiveObjectFlags.NotSpawned,
            PrimitiveObject.ColliderCreationMode.ServerOnlyNonSpawned => PrimitiveObjectFlags.ServerCollidable | PrimitiveObjectFlags.NotSpawned,
            PrimitiveObject.ColliderCreationMode.NoColliderNonSpawned => PrimitiveObjectFlags.NotSpawned,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown collider creation mode")
        };

    }

}
