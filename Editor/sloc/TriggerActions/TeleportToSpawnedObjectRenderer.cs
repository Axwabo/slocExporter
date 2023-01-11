using Editor.sloc.TriggerActions.Renderers;
using slocExporter.Objects;
using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions {

    public sealed class TeleportToSpawnedObjectRenderer : ITriggerActionEditorRenderer, ISelectedGizmosDrawer {

        public void DrawGUI(TriggerAction instance) {
            var data = instance.tpToSpawnedObject;
            var go = EditorGUILayout.ObjectField(new GUIContent("Target"), data.go, typeof(GameObject), true) as GameObject;
            data.go = go;
            data.offset = EditorGUILayout.Vector3Field("Offset", data.offset);
            if (IsValidObject(go))
                EditorGUILayout.HelpBox("A green wire sphere gizmo is indicating the point to teleport to", MessageType.None);
            else
                EditorGUILayout.HelpBox("Target must be a spawnable primitive (with a light or valid mesh)", MessageType.Error);
        }

        public void DrawGizmos(TriggerAction instance) {
            var data = instance.tpToSpawnedObject;
            var go = data.go;
            if (!go)
                return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(go.transform.TransformPoint(data.offset), 0.2f);
        }

        private static bool IsValidObject(GameObject o)
            => o != null &&
               (o.TryGetComponent(out Light _) ||
                o.TryGetComponent(out MeshFilter mesh) && ObjectExporter.FindObjectType(mesh.sharedMesh.name) != ObjectType.None);

    }

}
