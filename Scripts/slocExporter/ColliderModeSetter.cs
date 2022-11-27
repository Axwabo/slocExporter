using slocExporter.Objects;
using UnityEngine;

namespace slocExporter {

    [DisallowMultipleComponent]
    public sealed class ColliderModeSetter : MonoBehaviour {

        private const string NoCollider = "No Collider";
        private const string ClientOnly = "Client-Only";
        private const string ServerOnly = "Server-Only Spawned Object";
        private const string Both = "Both";
        private const string Trigger = "Trigger";
        private const string NonSpawnedTrigger = "Non-Spawned Trigger";
        private const string ServerOnlyNonSpawnedObject = "Server-Only Non-Spawned Object";
        private const string NoColliderNonSpawnedObject = "Colliderless Non-Spawned Object";

        private const string DescriptionNoCollider = "The object will neither have a collider on the client nor on the server.";
        private const string DescriptionClientOnly = "The object will have a collider on the client but not on the server.";
        private const string DescriptionServerOnly = "The object will be spawned on the client.\nIt will only have a collider on the server.";
        private const string DescriptionBoth = "The object will have a collider on the client and on the server.";
        private const string DescriptionTrigger = "The object will have a trigger collider on the server but won't be have a collider on the client.";
        private const string DescriptionNonSpawnedTrigger = "The object will NOT be spawned on the client.\nIt will have a trigger collider on the server.";
        private const string DescriptionServerOnlyNonSpawnedObject = "The object will NOT be spawned on the client.\nIt will only have a collider on the server.";
        private const string DescriptionNoColliderNonSpawnedObject = "The object will NOT be spawned on the client.\nIt will neither have a collider on the client nor on the server.";
        private const string DescriptionUnset = "The default collider mode will be used if set in the headers; otherwise, it will use the BOTH mode.";
        private const string DescriptionHeaderUnset = "This mode will be used for all primitives that don't have a collider mode set.";

        public PrimitiveObject.ColliderCreationMode mode = PrimitiveObject.ColliderCreationMode.Unset;

        public static string ModeToString(PrimitiveObject.ColliderCreationMode mode) => mode switch {
            PrimitiveObject.ColliderCreationMode.NoCollider => NoCollider,
            PrimitiveObject.ColliderCreationMode.ClientOnly => ClientOnly,
            PrimitiveObject.ColliderCreationMode.ServerOnly => ServerOnly,
            PrimitiveObject.ColliderCreationMode.Both => Both,
            PrimitiveObject.ColliderCreationMode.Trigger => Trigger,
            PrimitiveObject.ColliderCreationMode.NonSpawnedTrigger => NonSpawnedTrigger,
            PrimitiveObject.ColliderCreationMode.ServerOnlyNonSpawned => ServerOnlyNonSpawnedObject,
            PrimitiveObject.ColliderCreationMode.NoColliderNonSpawned => NoColliderNonSpawnedObject,
            _ => "Unset"
        };

        public static PrimitiveObject.ColliderCreationMode StringToMode(string mode) => mode switch {
            NoCollider => PrimitiveObject.ColliderCreationMode.NoCollider,
            ClientOnly => PrimitiveObject.ColliderCreationMode.ClientOnly,
            ServerOnly => PrimitiveObject.ColliderCreationMode.ServerOnly,
            Both => PrimitiveObject.ColliderCreationMode.Both,
            Trigger => PrimitiveObject.ColliderCreationMode.Trigger,
            NonSpawnedTrigger => PrimitiveObject.ColliderCreationMode.NonSpawnedTrigger,
            ServerOnlyNonSpawnedObject => PrimitiveObject.ColliderCreationMode.ServerOnlyNonSpawned,
            NoColliderNonSpawnedObject => PrimitiveObject.ColliderCreationMode.NoColliderNonSpawned,
            _ => PrimitiveObject.ColliderCreationMode.Unset
        };

        public static string GetModeDescription(PrimitiveObject.ColliderCreationMode mode, bool isHeader = false) => mode switch {
            PrimitiveObject.ColliderCreationMode.NoCollider => DescriptionNoCollider,
            PrimitiveObject.ColliderCreationMode.ClientOnly => DescriptionClientOnly,
            PrimitiveObject.ColliderCreationMode.ServerOnly => DescriptionServerOnly,
            PrimitiveObject.ColliderCreationMode.Both => DescriptionBoth,
            PrimitiveObject.ColliderCreationMode.Trigger => DescriptionTrigger,
            PrimitiveObject.ColliderCreationMode.NonSpawnedTrigger => DescriptionNonSpawnedTrigger,
            PrimitiveObject.ColliderCreationMode.ServerOnlyNonSpawned => DescriptionServerOnlyNonSpawnedObject,
            PrimitiveObject.ColliderCreationMode.NoColliderNonSpawned => DescriptionNoColliderNonSpawnedObject,
            _ => isHeader ? DescriptionHeaderUnset : DescriptionUnset
        };

    }

}
