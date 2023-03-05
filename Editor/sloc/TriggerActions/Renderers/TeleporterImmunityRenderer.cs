using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using slocExporter.TriggerActions.Enums;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class TeleporterImmunityRenderer : ITriggerActionEditorRenderer {

        private static readonly GUIContent DurationLabel = new("Duration Mode");

        private static readonly GUIContent[] DurationContent = {
            new("Absolute", "The current duration will be overwritten with the specified amount of time."),
            new("Add", "The given time is added to the current immunity time."),
            new("Subtract", "The given time is subtracted from the current immunity time.")
        };

        public string Description => "Makes the player immune to teleporters for a specified amount of time.\nIf set to global, they will be immune to all teleporters; otherwise just the one that triggered this action.";

        public void DrawGUI(TriggerAction instance) {
            var data = instance.tpImmunity;
            data.IsGlobal = EditorGUILayout.Toggle("Is Global", data.IsGlobal);
            data.DurationMode = (ImmunityDurationMode) EditorGUILayout.Popup(DurationLabel, (int) data.DurationMode, DurationContent);
            data.Duration = EditorGUILayout.Slider("Duration (seconds)", data.Duration, 0, TeleporterImmunityData.MaxValue);
        }

    }

}
