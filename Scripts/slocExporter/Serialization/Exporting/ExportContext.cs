using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting
{

    public sealed class ExportContext
    {

        public static ExportContext From(ExportPreset preset)
        {
            var attributes = slocAttributes.None;
            if (preset.lossyColors)
                attributes |= slocAttributes.LossyColors;
            if (preset.exportAllTriggerActions)
                attributes |= slocAttributes.ExportAllTriggerActions;
            if (preset.defaultPrimitiveFlags != PrimitiveObjectFlags.None)
                attributes |= slocAttributes.DefaultFlags;
            return new ExportContext(attributes, preset.defaultPrimitiveFlags);
        }

        public readonly slocAttributes Attributes;

        public readonly PrimitiveObjectFlags DefaultPrimitiveFlags;

        public ExportContext(slocAttributes attributes, PrimitiveObjectFlags defaultPrimitiveFlags)
        {
            Attributes = attributes;
            DefaultPrimitiveFlags = defaultPrimitiveFlags;
        }

    }

}
