using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class KillPlayerRenderer : ITriggerActionEditorRenderer {

        public string Description => "Kills the player with a specified cause.";

        public void DrawGUI(TriggerAction instance) {
            GUILayout.Label("Death Cause:");
            var i = instance.killPlayer;
            var input = EditorGUILayout.TextArea(i.Cause);
            if (input == i.Cause)
                return;
            Undo.RecordObject(instance, "Change Death Cause");
            i.Cause = input;
        }

    }

}
