using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    public sealed class MoveRelativeToSelfData : TeleportToPositionData {

        public override TriggerActionType ActionType => TriggerActionType.MoveRelativeToSelf;

        public MoveRelativeToSelfData(Vector3 position) : base(position) {
        }

    }

}
