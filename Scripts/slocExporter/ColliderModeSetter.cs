using slocExporter.Objects;
using UnityEngine;

namespace slocExporter {

    [DisallowMultipleComponent]
    public sealed class ColliderModeSetter : MonoBehaviour {

        public PrimitiveObject.ColliderCreationMode mode = PrimitiveObject.ColliderCreationMode.Unset;

        public static string ModeToString(PrimitiveObject.ColliderCreationMode mode) => mode switch {
            PrimitiveObject.ColliderCreationMode.NoCollider => "No Collider",
            PrimitiveObject.ColliderCreationMode.ClientOnly => "Client Only",
            PrimitiveObject.ColliderCreationMode.ServerOnly => "Server Only",
            PrimitiveObject.ColliderCreationMode.Both => "Both",
            PrimitiveObject.ColliderCreationMode.Trigger => "Trigger",
            PrimitiveObject.ColliderCreationMode.ServerOnlyTrigger => "Server-Only Trigger",
            _ => "Unset"
        };

        public static PrimitiveObject.ColliderCreationMode StringToMode(string mode) => mode switch {
            "No Collider" => PrimitiveObject.ColliderCreationMode.NoCollider,
            "Client Only" => PrimitiveObject.ColliderCreationMode.ClientOnly,
            "Server Only" => PrimitiveObject.ColliderCreationMode.ServerOnly,
            "Both" => PrimitiveObject.ColliderCreationMode.Both,
            "Trigger" => PrimitiveObject.ColliderCreationMode.Trigger,
            "Server-Only Trigger" => PrimitiveObject.ColliderCreationMode.ServerOnlyTrigger,
            _ => PrimitiveObject.ColliderCreationMode.Unset
        };

    }

}
