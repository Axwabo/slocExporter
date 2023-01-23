using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class KillPlayerRenderer : ITriggerActionEditorRenderer {

        public void DrawGUI(TriggerAction instance) {
            GUILayout.Label("Death Cause:");
            instance.killPlayer.Cause = EditorGUILayout.TextArea(instance.killPlayer.Cause);
        }

    }

}
