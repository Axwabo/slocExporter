using System.Collections.Generic;
using slocExporter.Extensions;
using slocExporter.Serialization.Exporting.Identifiers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace slocExporter.Serialization.Exporting
{

    public static class ExportCollector
    {

        public static HashSet<GameObject> GetObjects(bool selectedOnly, ExportPreset preset, bool debug)
        {
            var set = new HashSet<GameObject>();
            var initial = selectedOnly ? Selection.gameObjects : SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var gameObject in initial)
                TraverseChildren(set, gameObject, gameObject.transform, preset, debug);
            return set;
        }

        private static void TraverseChildren(HashSet<GameObject> set, GameObject gameObject, Transform transform, ExportPreset preset, bool debug)
        {
            if (gameObject.IsChildOfKnownPrefab())
            {
                if (debug)
                    Debug.Log("Skipping object as it's part of an SL prefab", gameObject);
                return;
            }

            if (!set.ConditionalAdd(gameObject, preset))
            {
                if (debug)
                    Debug.Log("Skipping object", gameObject);
                return;
            }

            var count = transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = transform.GetChild(i);
                TraverseChildren(set, child.gameObject, child, preset, debug);
            }
        }

        private static bool ConditionalAdd(this HashSet<GameObject> set, GameObject gameObject, ExportPreset preset)
            => !gameObject.IsIgnored() && preset.ShouldInclude(gameObject) && set.Add(gameObject);

        public static bool ShouldInclude(this ExportPreset preset, GameObject gameObject)
            => (gameObject.activeSelf || preset.includeInactiveObjects)
               && (preset.traversePrefabs || !gameObject.IsChildOfAnyPrefab(true, out _));

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
