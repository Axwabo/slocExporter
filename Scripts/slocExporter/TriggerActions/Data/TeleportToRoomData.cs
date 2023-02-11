using System.IO;
using slocExporter.TriggerActions.Enums;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    public sealed class TeleportToRoomData : BaseTriggerActionData {

        public override TargetType PossibleTargets => TargetType.All;

        public override TriggerActionType ActionType => TriggerActionType.TeleportToRoom;

        [field: SerializeField]
        public Vector3 Offset { get; set; }

        [field: SerializeField]
        public string Room { get; set; }

        public TeleportToRoomData(string room, Vector3 offset) {
            Room = room;
            Offset = offset;
        }

        protected override void WriteData(BinaryWriter writer) {
            writer.Write(Room);
            writer.WriteVector(Offset);
        }

    }

}
