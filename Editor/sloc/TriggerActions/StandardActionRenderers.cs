using slocExporter.TriggerActions;
using UnityEditor;

namespace Editor.sloc.TriggerActions {

    public static class StandardActionRenderers {

        public static void DrawTeleportToPosition(TriggerAction instance) =>
            instance.tpToPos.position = EditorGUILayout.Vector3Field("Position", instance.tpToPos.position);

    }

}
