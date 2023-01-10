using System.IO;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    public sealed class TeleportToRoomData : BaseTriggerActionData {

        public override TargetType TargetType => TargetType.All;

        public override TriggerActionType ActionType => TriggerActionType.TeleportToRoom;
        
        public readonly Vector3 PositionOffset;

        public readonly string Room;

        public TeleportToRoomData(Vector3 positionOffset, string room) {
            PositionOffset = positionOffset;
            Room = room;
        }

        protected override void WriteData(BinaryWriter writer) {
            writer.WriteVector(PositionOffset);
            writer.Write(Room);
        }

    }

}
