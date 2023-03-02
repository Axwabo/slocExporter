﻿using System.IO;
using slocExporter.TriggerActions.Enums;
using UnityEngine;

namespace slocExporter.TriggerActions.Data {

    public sealed class TeleporterImmunityData : BaseTriggerActionData {

        public const float MaxValue = 60f;
        public const float FloatToShortMultiplier = 1000f;
        public const float ShortToFloatMultiplier = 1f / FloatToShortMultiplier;

        public override TriggerActionType ActionType => TriggerActionType.TeleporterImmunity;

        public override TargetType PossibleTargets => TargetType.Player;

        [field: SerializeField]
        public bool IsGlobal { get; set; }

        [field: SerializeField]
        public float Duration { get; set; }

        public TeleporterImmunityData(bool isGlobal, float duration) {
            IsGlobal = isGlobal;
            Duration = duration;
        }

        protected override void WriteData(BinaryWriter writer) {
            writer.Write(IsGlobal);
            writer.WriteFloatAsShort(Duration);
        }

    }

}
