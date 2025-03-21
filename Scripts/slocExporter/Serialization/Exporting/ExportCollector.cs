using System.Collections.Generic;
using slocExporter.Serialization.Exporting.Identifiers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace slocExporter.Serialization.Exporting
{

    public static class ExportCollector
    {

        public static HashSet<GameObject> GetObjects(bool selectedOnly)
        {
            var set = new HashSet<GameObject>();
            var initial = selectedOnly ? Selection.gameObjects : SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var gameObject in initial)
                TranverseChildren(set, gameObject, gameObject.transform);
            return set;
        }

        private static void TranverseChildren(HashSet<GameObject> set, GameObject gameObject, Transform transform)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(gameObject) && PrefabStructureIdentifier.Instance.Process(gameObject) != null
                || !set.Add(gameObject))
                return;
            var count = transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = transform.GetChild(i);
                var go = child.gameObject;
                if (set.Add(go))
                    TranverseChildren(set, go, child);
            }
        }

    }

}
