using System;
using System.IO;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    [Serializable]
    public abstract class BaseTriggerActionData {

        [field: SerializeField]
        private TargetType Selected { get; set; }

        public abstract TargetType PossibleTargets { get; }

        public abstract TriggerActionType ActionType { get; }

        public TargetType SelectedTargets {
            get => Selected;
            set {
                if (value is TargetType.None) {
                    Selected = value;
                    return;
                }

                var possible = PossibleTargets;
                foreach (var v in ActionManager.TargetTypeValues)
                    if (value.HasFlagFast(v) && possible.HasFlagFast(v))
                        Selected |= v;
                    else
                        Selected &= ~v;
            }
        }

        protected BaseTriggerActionData() => SelectedTargets = TargetType.All;

        public void WriteTo(BinaryWriter writer) {
            writer.Write((ushort) ActionType);
            writer.Write((byte) SelectedTargets);
            WriteData(writer);
        }

        protected virtual void WriteData(BinaryWriter writer) {
        }

    }

}
