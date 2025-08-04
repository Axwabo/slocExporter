using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting
{

    public readonly struct ExportContext
    {

        public static ExportContext From(ExportPreset preset, bool debug = false)
        {
            var attributes = slocAttributes.None;
            if (preset.lossyColors)
                attributes |= slocAttributes.LossyColors;
            if (preset.defaultPrimitiveFlags != PrimitiveObjectFlags.None)
                attributes |= slocAttributes.DefaultFlags;
            if (preset.exportAllTriggerActions)
                attributes |= slocAttributes.ExportAllTriggerActions;
            if (preset.exportNamesAndTags)
                attributes |= slocAttributes.NamesAndTags;
            return new ExportContext(attributes, preset.defaultPrimitiveFlags, debug);
        }

        public readonly slocAttributes Attributes;

        public readonly PrimitiveObjectFlags DefaultPrimitiveFlags;

        public readonly bool Debug;

        public ExportContext(slocAttributes attributes, PrimitiveObjectFlags defaultPrimitiveFlags, bool debug = false)
        {
            Attributes = attributes;
            DefaultPrimitiveFlags = defaultPrimitiveFlags;
            Debug = debug;
        }

    }

}
