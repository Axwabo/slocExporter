using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Identifiers;

namespace slocExporter.Serialization.Exporting
{

    public static class IdentifierCache
    {

        public static readonly IObjectIdentifier<IExportable<slocGameObject>>[] Identifiers =
        {
            new OverriddenStructureIdentifier(),
            new PrefabStructureIdentifier()
        };

    }

}
