using System;
using System.IO;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    [Serializable]
    public class TeleportToPositionData : BaseTriggerActionData {

        public sealed override TargetType TargetType => TargetType.All;

        public override TriggerActionType ActionType => TriggerActionType.TeleportToPosition;

        public Vector3 position;

        public TeleportToPositionData(Vector3 position) => this.position = position;

        protected override void WriteData(BinaryWriter writer) => writer.WriteVector(position);

    }

}
