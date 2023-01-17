﻿using slocExporter.TriggerActions.Data;
using UnityEngine;

namespace slocExporter.TriggerActions {

    public sealed class TriggerAction : MonoBehaviour {

        public static TriggerActionGizmosDrawer CurrentGizmosDrawer = null;

        public TriggerActionType type;

        [SerializeReference]
        public TeleportToPositionData tpToPos;

        [SerializeReference]
        public TeleportToRoomData tpToRoom;

        [SerializeReference]
        public RuntimeTeleportToSpawnedObjectData tpToSpawnedObject;

        [SerializeReference]
        public MoveRelativeToSelfData moveRel;

        [SerializeReference]
        public KillPlayerData killPlayer;

        public BaseTriggerActionData SelectedData => type switch {
            TriggerActionType.TeleportToPosition => tpToPos,
            TriggerActionType.TeleportToRoom => tpToRoom,
            TriggerActionType.TeleportToSpawnedObject => tpToSpawnedObject,
            TriggerActionType.MoveRelativeToSelf => moveRel,
            TriggerActionType.KillPlayer => killPlayer,
            _ => null
        };

        public void SetData(BaseTriggerActionData data) {
            switch (type = data.ActionType) {
                case TriggerActionType.TeleportToPosition:
                    tpToPos = data as TeleportToPositionData;
                    break;
                case TriggerActionType.TeleportToRoom:
                    tpToRoom = data as TeleportToRoomData;
                    break;
                case TriggerActionType.MoveRelativeToSelf:
                    moveRel = data as MoveRelativeToSelfData;
                    break;
                case TriggerActionType.KillPlayer:
                    killPlayer = data as KillPlayerData;
                    break;
                default:
                    Debug.LogWarning($"Trigger action type \"{type}\" cannot be processed automatically (failing GameObject: \"{gameObject.name}\")");
                    break;
            }
        }

        private void OnDrawGizmosSelected() => CurrentGizmosDrawer?.Invoke(this);

    }

}
