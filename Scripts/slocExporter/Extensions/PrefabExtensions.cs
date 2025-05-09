using UnityEditor;
using UnityEngine;

namespace slocExporter.Extensions
{

    public static class PrefabExtensions
    {

        public static bool TryGetPrefabGuid(this GameObject o, out GUID guid)
        {
            if (!PrefabUtility.IsOutermostPrefabInstanceRoot(o))
            {
                guid = default;
                return false;
            }

            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(o);
            if (!prefab)
            {
                guid = default;
                return false;
            }

            guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(prefab));
            return true;
        }

    }

}
