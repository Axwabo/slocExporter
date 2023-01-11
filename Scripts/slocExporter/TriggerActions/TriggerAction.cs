using slocExporter.TriggerActions.Data;
using UnityEngine;

namespace slocExporter.TriggerActions {

    public sealed class TriggerAction : MonoBehaviour {

        public static TriggerActionGizmosDrawer CurrentGizmosDrawer = null;

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

        public void SetData(BaseTriggerActionData data) {
            type = data.ActionType;
            switch (type) {
                case TriggerActionType.TeleportToPosition:
                    tpToPos = data as TeleportToPositionData;
                    break;
                case TriggerActionType.TeleportToRoom:
                    tpToRoom = data as TeleportToRoomData;
                    break;
                // case TriggerActionType.TeleportToSpawnedObject:
                    // tpToSpawnedObject = ObjectExporter.ProcessSerializedTpToSpawnedObject(data as SerializableTeleportToSpawnedObjectData);
                    // break;
                case TriggerActionType.MoveRelativeToSelf:
                    moveRel = data as MoveRelativeToSelfData;
                    break;
                case TriggerActionType.KillPlayer:
                    killPlayer = data as KillPlayerData;
                    break;
                default:
                    Debug.LogWarning("Unknown trigger action type: " + type);
                    break;
            }
        }

        private void OnDrawGizmosSelected() => CurrentGizmosDrawer?.Invoke(this);

    }

}
