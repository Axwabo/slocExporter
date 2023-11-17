using System.Collections.Generic;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using slocExporter.TriggerActions.Enums;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers
{

    public sealed class SimplePositionRenderer : ITriggerActionEditorRenderer
    {

        private static readonly Dictionary<TeleportOptions, GUIContent> TeleportOptionsContent = new()
        {
            {TeleportOptions.ResetFallDamage, new GUIContent("Reset Fall Damage", "Resets fall damage on the target player.")},
            {TeleportOptions.ResetVelocity, new GUIContent("Reset Velocity", "Resets the target player's velocity.")},
            {TeleportOptions.WorldSpaceTransform, new GUIContent("World Space Transform", "Uses world-space transform to calculate the offset instead of relative calculation based on the object this action is added to.")}
        };

        public delegate BaseTeleportData PositionGetter(TriggerAction instance);

        public string Description { get; }

        private readonly string _label;
        private readonly PositionGetter _positionGetter;

        public SimplePositionRenderer(string label, PositionGetter positionGetter, string description)
        {
            _label = label;
            _positionGetter = positionGetter;
            Description = description;
        }

        public void DrawGUI(TriggerAction instance)
        {
            var i = _positionGetter(instance);
            var input = EditorGUILayout.Vector3Field(_label, i.Position);
            if (input != i.Position)
            {
                Undo.RecordObject(instance, "Change Teleport Offset");
                i.Position = input;
            }

            DrawCheckboxes(instance, i);
        }

        public static void DrawCheckboxes(TriggerAction triggerAction, BaseTeleportData data)
        {
            var value = data.Options;
            foreach (var type in ActionManager.TeleportOptionsValues)
            {
                var content = TeleportOptionsContent.GetValueOrDefault(type) ?? new GUIContent(type.ToString());
                var active = EditorGUILayout.Toggle(content, value.HasFlagFast(type));
                if (active)
                    value |= type;
                else
                    value &= ~type;
            }

            if (value == data.Options)
                return;
            EditorUtility.SetDirty(triggerAction);
            Undo.RecordObject(triggerAction, "Change Teleport Options");
            data.Options = value;
        }

    }

}
