using Editor.sloc.TriggerActions.Renderers;

namespace Editor.sloc.TriggerActions
{

    public static class ActionRenderers
    {

        public static readonly SimplePositionRenderer TeleportToPosition = new(
            "Absolute Position",
            i => i.tpToPos,
            "Teleports the object to a static position on the map."
        );

        public static readonly SimplePositionRenderer MoveRelativeToSelf = new(
            "Relative Position",
            i => i.moveRel,
            "Moves the interacting object relative to its current position."
        );

        public static readonly TeleportToRoomRenderer TeleportToRoom = new();

        public static readonly KillPlayerRenderer KillPlayer = new();

        public static readonly TeleportToSpawnedObjectRenderer TeleportToSpawnedObject = new();

        public static readonly TeleporterImmunityRenderer TeleporterImmunity = new();

    }

}
