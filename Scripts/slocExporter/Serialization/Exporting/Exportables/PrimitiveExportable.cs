using System.Collections.Generic;
using slocExporter.Objects;
using slocExporter.TriggerActions.Data;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class PrimitiveExportable : IExportable<PrimitiveObject>
    {

        public ObjectType PrimitiveType;

        public PrimitiveObjectFlags ResolvedFlags;

        public PrimitiveObjectFlags OverriddenFlags;

        public Color MaterialColor;

        public readonly List<BaseTriggerActionData> TriggerActions = new();

        public PrimitiveObject Export(int instanceId) => new(instanceId, PrimitiveType)
        {
            MaterialColor = MaterialColor,
            TriggerActions = TriggerActions.ToArray(),
            Flags = OverriddenFlags != PrimitiveObjectFlags.None
                ? OverriddenFlags
                : ResolvedFlags
        };

    }

}
