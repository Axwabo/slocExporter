﻿using System;

namespace slocExporter.TriggerActions.Enums
{

    [Flags]
    public enum TeleportOptions : byte
    {

        None = 0,
        ResetFallDamage = 1,
        ResetVelocity = 2,
        WorldSpaceTransform = 4,
        DeltaRotation = 8,
        All = ResetFallDamage | ResetVelocity | WorldSpaceTransform | DeltaRotation

    }

}
