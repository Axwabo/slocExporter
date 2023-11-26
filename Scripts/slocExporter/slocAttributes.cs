using System;

namespace slocExporter
{

    [Flags]
    public enum slocAttributes : byte
    {

        None = 0,
        LossyColors = 1,
        DefaultColliderMode = 2,
        ExportAllTriggerActions = 4

    }

}
