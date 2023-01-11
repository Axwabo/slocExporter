using System;
using System.IO;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    [Serializable]
    public sealed class MoveRelativeToSelfData : BaseTriggerActionData {

        public sealed override TargetType TargetType => TargetType.All;

        public override TriggerActionType ActionType => TriggerActionType.MoveRelativeToSelf;

        public Vector3 position;

        public MoveRelativeToSelfData(Vector3 position) => this.position = position;

        protected override void WriteData(BinaryWriter writer) => writer.WriteVector(position);

    }

}
