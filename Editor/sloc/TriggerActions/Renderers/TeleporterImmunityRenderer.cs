using System;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using slocExporter.TriggerActions.Enums;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers
{

    public sealed class TeleporterImmunityRenderer : ITriggerActionEditorRenderer
    {

        private static readonly GUIContent DurationLabel = new("Duration Mode");

        private static readonly GUIContent[] DurationContent =
        {
            new("Absolute", "The current duration will be overwritten with the specified amount of time."),
            new("Add", "The given time is added to the current immunity time."),
            new("Subtract", "The given time is subtracted from the current immunity time.")
        };

        public string Description => "Makes the player immune to teleporters for a specified amount of time.\nIf set to global, they will be immune to all teleporters; otherwise just the one that triggered this action.";

        public void DrawGUI(TriggerAction instance)
        {
            var data = instance.tpImmunity;
            var global = EditorGUILayout.Toggle("Is Global", data.IsGlobal);
            if (global != data.IsGlobal)
            {
                Undo.RecordObject(instance, "Change Immunity Global Mode");
                data.IsGlobal = global;
            }

            var mode = (ImmunityDurationMode) EditorGUILayout.Popup(DurationLabel, (int) data.DurationMode, DurationContent);
            if (mode != data.DurationMode)
            {
                Undo.RecordObject(instance, "Change Immunity Duration Mode");
                data.DurationMode = mode;
            }

            var duration = EditorGUILayout.Slider("Duration (seconds)", data.Duration, 0, TeleporterImmunityData.MaxValue);
            if (Math.Abs(duration - data.Duration) < 0.001f)
                return;
            Undo.RecordObject(instance, "Change Immunity Duration Time");
            data.Duration = duration;
        }

    }

}
