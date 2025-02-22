using slocExporter.Serialization.Exporting.Exportables;

namespace slocExporter.Serialization.Exporting.Processors
{

    public static class PrimitiveFlagsSetterProcessor
    {

        public static void Process(PrimitiveExportable primitive, PrimitiveFlagsSetter setter) => primitive.OverriddenFlags = setter.flags;

    }

}
