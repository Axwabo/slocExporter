using slocExporter.Objects;
using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class TeleportToSpawnedObjectRenderer : ITriggerActionEditorRenderer, ISelectedGizmosDrawer {

        public void DrawGUI(TriggerAction instance) {
            var data = instance.tpToSpawnedObject;
            var go = EditorGUILayout.ObjectField(new GUIContent("Target Object","The object to teleport to."), data.Target, typeof(GameObject), true) as GameObject;
            data.Target = go;
            data.Offset = EditorGUILayout.Vector3Field("Offset", data.Offset);
            if (IsValidObject(go))
                EditorGUILayout.HelpBox("A green wire sphere gizmo is indicating the point to teleport to", MessageType.None);
            else
                EditorGUILayout.HelpBox("Target must be a spawnable primitive (with a light or valid mesh)", MessageType.Error);
        }

        public void DrawGizmos(TriggerAction instance) {
            var data = instance.tpToSpawnedObject;
            var go = data.Target;
            if (!go)
                return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(go.transform.TransformPoint(data.Offset), 0.2f);
        }

        private static bool IsValidObject(GameObject o)
            => o != null &&
               (o.TryGetComponent(out Light _) ||
                o.TryGetComponent(out MeshFilter mesh) && ObjectExporter.FindObjectType(mesh.sharedMesh.name) != ObjectType.None);

    }

}
