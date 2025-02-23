using slocExporter.Extensions;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEditor;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class PrefabStructureIdentifier : IObjectIdentifier<StructureExportable>
    {

        public static readonly PrefabStructureIdentifier Instance = new();

        public StructureExportable Process(GameObject o)
        {
            if (!PrefabUtility.IsOutermostPrefabInstanceRoot(o))
                return null;
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(o);
            if (!prefab)
                return null;
            var guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(prefab)).ToString();
            return Identify.StructureGuids.TryGetValue(guid, out var type)
                ? new StructureExportable {StructureType = type}
                : null;
        }

    }

}
