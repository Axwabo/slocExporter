using System;
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
            {TeleportOptions.ResetVelocity, new GUIContent("Reset Velocity", "Resets the target object's velocity. Applies to items and ragdolls only.")},
            {TeleportOptions.WorldSpaceTransform, new GUIContent("World Space Transform", "Uses world-space transform to calculate the offset instead of relative calculation based on the object this action is added to.")},
            {TeleportOptions.DeltaRotation, new GUIContent("Delta Rotation", "If enabled, the object will be rotated on the Y axis by the specified amount, ignoring world-space transform.")}
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

        public void DrawGUI(TriggerAction instance) => DrawCommonElements(instance, _positionGetter(instance), _label);

        public static void DrawCommonElements(TriggerAction triggerAction, BaseTeleportData data, string label = "Position Offset")
        {
            var position = EditorGUILayout.Vector3Field(label, data.Position);
            if (position != data.Position)
            {
                Undo.RecordObject(triggerAction, "Change Teleport Offset");
                data.Position = position;
            }

            var rotation = EditorGUILayout.FloatField("Y Rotation", data.RotationY);
            if ((Math.Abs(rotation - data.RotationY) > 0.001f))
            {
                Undo.RecordObject(triggerAction, "Change Teleport Rotation");
                data.RotationY = rotation;
            }

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
