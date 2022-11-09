using slocExporter.Objects;
using UnityEngine;

namespace slocExporter {

    [DisallowMultipleComponent]
    public sealed class ColliderModeSetter : MonoBehaviour {

        private const string NoCollider = "No Collider";
        private const string ClientOnly = "Client Only";
        private const string ServerOnlySpawnedObject = "Server-Only Spawned Object";
        private const string Both = "Both";
        private const string Trigger = "Trigger";
        private const string ServerOnlyTrigger = "Server-Only Trigger";
        private const string ServerOnlyNonSpawnedObject = "Server-Only Non-Spawned Object";

        private const string DescriptionNoCollider = "The object will neither have a collider on the client nor on the server.";
        private const string DescriptionClientOnly = "The object will have a collider on the client but not on the server.";
        private const string DescriptionServerOnlySpawnedObject = "The object will be spawned on the client.\nIt will only have a collider on the server.";
        private const string DescriptionBoth = "The object will have a collider on the client and on the server.";
        private const string DescriptionTrigger = "The object will have a trigger collider on the server but won't be have a collider on the client.";
        private const string DescriptionServerOnlyTrigger = "The object will NOT be spawned on the client.\nIt will have a trigger collider on the server.";
        private const string DescriptionServerOnlyNonSpawnedObject = "The object will NOT be spawned on the client.\nIt will only have a collider on the server.";
        private const string DescriptionUnset = "The default collider mode will be used if set in the headers; otherwise, it will use the BOTH mode.";
        private const string DescriptionHeaderUnset = "This mode will be used for all primitives that don't have a collider mode set.";

        public PrimitiveObject.ColliderCreationMode mode = PrimitiveObject.ColliderCreationMode.Unset;

        public static string ModeToString(PrimitiveObject.ColliderCreationMode mode) => mode switch {
            PrimitiveObject.ColliderCreationMode.NoCollider => NoCollider,
            PrimitiveObject.ColliderCreationMode.ClientOnly => ClientOnly,
            PrimitiveObject.ColliderCreationMode.ServerOnlySpawned => ServerOnlySpawnedObject,
            PrimitiveObject.ColliderCreationMode.Both => Both,
            PrimitiveObject.ColliderCreationMode.Trigger => Trigger,
            PrimitiveObject.ColliderCreationMode.ServerOnlyTrigger => ServerOnlyTrigger,
            PrimitiveObject.ColliderCreationMode.ServerOnlyNonSpawned => ServerOnlyNonSpawnedObject,
            _ => "Unset"
        };

        public static PrimitiveObject.ColliderCreationMode StringToMode(string mode) => mode switch {
            NoCollider => PrimitiveObject.ColliderCreationMode.NoCollider,
            ClientOnly => PrimitiveObject.ColliderCreationMode.ClientOnly,
            ServerOnlySpawnedObject => PrimitiveObject.ColliderCreationMode.ServerOnlySpawned,
            Both => PrimitiveObject.ColliderCreationMode.Both,
            Trigger => PrimitiveObject.ColliderCreationMode.Trigger,
            ServerOnlyTrigger => PrimitiveObject.ColliderCreationMode.ServerOnlyTrigger,
            ServerOnlyNonSpawnedObject => PrimitiveObject.ColliderCreationMode.ServerOnlyNonSpawned,
            _ => PrimitiveObject.ColliderCreationMode.Unset
        };

        public static string GetModeDescription(PrimitiveObject.ColliderCreationMode mode, bool isHeader = false) => mode switch {
            PrimitiveObject.ColliderCreationMode.NoCollider => DescriptionNoCollider,
            PrimitiveObject.ColliderCreationMode.ClientOnly => DescriptionClientOnly,
            PrimitiveObject.ColliderCreationMode.ServerOnlySpawned => DescriptionServerOnlySpawnedObject,
            PrimitiveObject.ColliderCreationMode.Both => DescriptionBoth,
            PrimitiveObject.ColliderCreationMode.Trigger => DescriptionTrigger,
            PrimitiveObject.ColliderCreationMode.ServerOnlyTrigger => DescriptionServerOnlyTrigger,
            PrimitiveObject.ColliderCreationMode.ServerOnlyNonSpawned => DescriptionServerOnlyNonSpawnedObject,
            _ => isHeader ? DescriptionHeaderUnset : DescriptionUnset
        };

    }

}
