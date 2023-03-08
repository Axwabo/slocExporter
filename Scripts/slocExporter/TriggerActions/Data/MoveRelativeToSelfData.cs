using slocExporter.TriggerActions.Enums;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    public sealed class MoveRelativeToSelfData : BaseTeleportData {

        public override TargetType PossibleTargets => TargetType.All;

        public override TriggerActionType ActionType => TriggerActionType.MoveRelativeToSelf;

        public MoveRelativeToSelfData(Vector3 offset) => Position = offset;

    }

}
