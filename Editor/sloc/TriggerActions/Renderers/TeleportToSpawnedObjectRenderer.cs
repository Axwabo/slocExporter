using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers
{

    public sealed class TeleportToSpawnedObjectRenderer : ITriggerActionEditorRenderer, ISelectedGizmosDrawer
    {

        private static readonly GUIContent Content = new("Target Object", "The object to teleport to.");

        public string Description => "Teleports the object to another specified object with the given position offset.";

        public void DrawGUI(TriggerAction instance)
        {
            var data = instance.tpToSpawnedObject;
            var go = EditorGUILayout.ObjectField(Content, data.Target, typeof(GameObject), true) as GameObject;
            if (data.Target != go)
            {
                EditorUtility.SetDirty(instance);
                Undo.RecordObject(instance, "Change Teleport Target");
                data.Target = go;
            }

            SimplePositionRenderer.DrawCommonElements(instance, data);
            if (IsValidObject(go))
                EditorGUILayout.HelpBox("A green wire sphere gizmo is indicating the point to teleport to", MessageType.Info);
            else
                EditorGUILayout.HelpBox("Target must be a spawnable primitive (with a light or valid mesh)", MessageType.Error);
        }

        public void DrawGizmos(TriggerAction instance)
        {
            var data = instance.tpToSpawnedObject;
            var go = data?.Target;
            if (!go)
                return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(data.ToWorldSpacePosition(go.transform), 0.2f);
        }

        private static bool IsValidObject(GameObject o)
            => o != null &&
               (o.TryGetComponent(out Light _) ||
                o.TryGetComponent(out MeshFilter mesh) && Identify.PrimitiveObjectType(mesh.sharedMesh.name) != ObjectType.None);

    }

}
