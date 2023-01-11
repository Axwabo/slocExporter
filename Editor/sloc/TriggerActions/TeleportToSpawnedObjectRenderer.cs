using slocExporter.Objects;
using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions {

    public static class TeleportToSpawnedObjectRenderer {

        public static void Draw(TriggerAction instance) {
            var data = instance.tpToSpawnedObject;
            var go = EditorGUILayout.ObjectField(new GUIContent("Target"), data.go, typeof(GameObject), true) as GameObject;
            data.go = go;
            data.offset = EditorGUILayout.Vector3Field("Offset", data.offset);
            if (!IsValidObject(go)) 
                EditorGUILayout.HelpBox("Target must be a spawnable primitive (with a light or valid mesh)", MessageType.Error);
        }

        private static bool IsValidObject(GameObject o)
            => o.TryGetComponent(out Light _) ||
               o.TryGetComponent(out MeshFilter mesh) && ObjectExporter.FindObjectType(mesh.sharedMesh.name) != ObjectType.None;

    }

}
