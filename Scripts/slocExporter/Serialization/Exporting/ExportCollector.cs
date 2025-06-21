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

        public static HashSet<GameObject> GetObjects(bool selectedOnly, ExportPreset preset)
        {
            var set = new HashSet<GameObject>();
            var initial = selectedOnly ? Selection.gameObjects : SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var gameObject in initial)
                TraverseChildren(set, gameObject, gameObject.transform, preset);
            return set;
        }

        private static void TraverseChildren(HashSet<GameObject> set, GameObject gameObject, Transform transform, ExportPreset preset)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(gameObject) && PrefabStructureIdentifier.Instance.Process(gameObject) != null
                || !set.ConditionalAdd(gameObject, preset))
                return;
            var count = transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = transform.GetChild(i);
                var go = child.gameObject;
                if (set.ConditionalAdd(go, preset))
                    TraverseChildren(set, go, child, preset);
            }
        }

        private static bool ConditionalAdd(this HashSet<GameObject> set, GameObject gameObject, ExportPreset preset)
            => !gameObject.IsIgnored() && preset.ShouldInclude(gameObject) && set.Add(gameObject);

        public static bool ShouldInclude(this ExportPreset preset, GameObject gameObject)
            => (gameObject.activeSelf || preset.includeInactiveObjects)
               && (preset.traversePrefabs || !PrefabUtility.IsAnyPrefabInstanceRoot(gameObject));

    }

}
