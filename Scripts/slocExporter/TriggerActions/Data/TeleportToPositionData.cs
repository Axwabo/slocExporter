using System.IO;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    public class TeleportToPositionData : BaseTriggerActionData {

        public sealed override TargetType TargetType => TargetType.All;

        public override TriggerActionType ActionType => TriggerActionType.TeleportToPosition;

        public readonly Vector3 Position;

        public TeleportToPositionData(Vector3 position) => Position = position;

        protected override void WriteData(BinaryWriter writer) => writer.WriteVector(Position);

    }

}
