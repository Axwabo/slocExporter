using slocExporter.TriggerActions.Data;
using UnityEngine;

namespace slocExporter.TriggerActions {

    public sealed class TriggerAction : MonoBehaviour {

        public TriggerActionType type = TriggerActionType.None;

        public TeleportToPositionData tpToPos;

        public TeleportToRoomData tpToRoom;

        public SerializableTeleportToSpawnedObjectData tpToSpawnedObject;

        public MoveRelativeToSelfData moveRel;

        public KillPlayerData killPlayer;

        public BaseTriggerActionData SelectedData => type switch {
            TriggerActionType.TeleportToPosition => tpToPos,
            TriggerActionType.TeleportToRoom => tpToRoom,
            TriggerActionType.TeleportToSpawnedObject => ObjectExporter.ProcessSerializedTpToSpawnedObject(tpToSpawnedObject),
            TriggerActionType.MoveRelativeToSelf => moveRel,
            TriggerActionType.KillPlayer => killPlayer,
            _ => null
        };

    }

}
