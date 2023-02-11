using System.IO;
using slocExporter.TriggerActions.Enums;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    public sealed class RuntimeTeleportToSpawnedObjectData : BaseTeleportData {

        public override TargetType PossibleTargets => TargetType.All;

        public override TriggerActionType ActionType => TriggerActionType.TeleportToSpawnedObject;

        [field: SerializeField]
        public GameObject Target { get; set; }

        public RuntimeTeleportToSpawnedObjectData(GameObject target, Vector3 offset) {
            Target = target;
            Position = offset;
        }

        protected override void WriteAdditionalData(BinaryWriter writer) => writer.Write(Target.GetInstanceID());

    }

}
