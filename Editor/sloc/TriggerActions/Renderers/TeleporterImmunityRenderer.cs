using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEditor;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class TeleporterImmunityRenderer : ITriggerActionEditorRenderer {

        public string Description => "Makes the player immune to teleporters for a specified amount of time.\nIf set to global, they will be immune to all teleporters; otherwise just the one that triggered this action.";

        public void DrawGUI(TriggerAction instance) {
            var data = instance.tpImmunity;
            data.IsGlobal = EditorGUILayout.Toggle("Is Global", data.IsGlobal);
            data.Duration = EditorGUILayout.Slider("Duration (seconds)", data.Duration, 0, TeleporterImmunityData.MaxValue);
        }

    }

}
