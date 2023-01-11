using Editor.sloc.TriggerActions.Renderers;

namespace Editor.sloc.TriggerActions {

    public static class ActionRenderers {

        public static readonly SimplePositionRenderer TeleportToPosition = new(
            "Absolute Position",
            i => i.tpToPos.position,
            (i, v) => i.tpToPos.position = v
        );

        public static readonly SimplePositionRenderer MoveRelativeToSelf = new(
            "Relative Position",
            i => i.moveRel.offset,
            (i, v) => i.moveRel.offset = v
        );

        public static readonly TeleportToRoomRenderer TeleportToRoom = new();

        public static readonly KillPlayerRenderer KillPlayer = new();
        
        public static readonly TeleportToSpawnedObjectRenderer TeleportToSpawnedObject = new();

    }

}
