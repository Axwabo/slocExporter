using slocExporter.Serialization.Exporting.Identifiers;
using UnityEditor;
using UnityEngine;

namespace slocExporter.Extensions
{

    public static class PrefabExtensions
    {

        public static bool TryGetPrefabGuid(this GameObject o, out GUID guid, bool validateRoot = true)
        {
            if (validateRoot && !PrefabUtility.IsAnyPrefabInstanceRoot(o))
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

        public static bool TryLoadAsset(string guidString, out GameObject prefab)
        {
            if (GUID.TryParse(guidString, out var guid))
                return TryLoadAsset(guid, out prefab);
            prefab = null;
            return false;
        }

        public static bool TryLoadAsset(GUID guid, out GameObject prefab)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                prefab = null;
                return false;
            }

            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab != null;
        }

        public static GameObject InstantiatePrefab(this GameObject prefab)
            => (GameObject) PrefabUtility.InstantiatePrefab(prefab);

        public static bool IsChildOfAnyPrefab(this GameObject o, bool includeAddedObjects, out bool isRoot)
        {
            isRoot = PrefabUtility.IsAnyPrefabInstanceRoot(o);
            return !isRoot && (PrefabUtility.IsPartOfAnyPrefab(o) || includeAddedObjects && PrefabUtility.IsAddedGameObjectOverride(o));
        }

        public static bool IsChildOfKnownPrefab(this GameObject gameObject)
        {
            if (!gameObject.IsChildOfAnyPrefab(false, out var isRoot))
                return false;
            var root = isRoot ? gameObject : PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
            if (!root.TryGetPrefabGuid(out var guid, false))
                return false;
            if (guid == CapybaraIdentifier.CapybaraGuid)
                return true;
            var guidString = guid.ToString();
            return Identify.CameraGuids.ContainsKey(guidString) || Identify.StructureGuids.ContainsKey(guidString);
        }

    }

}
