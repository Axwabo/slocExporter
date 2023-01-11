using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class KillPlayerRenderer : ITriggerActionEditorRenderer {

        public void DrawGUI(TriggerAction instance) {
            GUILayout.Label("Death Cause:");
            instance.killPlayer.cause = EditorGUILayout.TextArea(instance.killPlayer.cause);
        }

    }

}
