using System;
using System.Collections.Generic;
using System.Linq;
using Editor.sloc.TriggerActions.Renderers;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions {

    [CustomEditor(typeof(TriggerAction))]
    public sealed class MainTriggerActionEditor : UnityEditor.Editor {

        private static readonly Dictionary<TriggerActionType, ITriggerActionEditorRenderer> Renderers = new() {
            {TriggerActionType.TeleportToPosition, ActionRenderers.TeleportToPosition},
            {TriggerActionType.MoveRelativeToSelf, ActionRenderers.MoveRelativeToSelf},
            {TriggerActionType.TeleportToRoom, ActionRenderers.TeleportToRoom},
            {TriggerActionType.KillPlayer, ActionRenderers.KillPlayer},
            {TriggerActionType.TeleportToSpawnedObject, ActionRenderers.TeleportToSpawnedObject}
        };

        private static readonly TriggerActionType[] Values = (TriggerActionType[]) Enum.GetValues(typeof(TriggerActionType));
        private static readonly string[] Names = Enum.GetNames(typeof(TriggerActionType));

        public override void OnInspectorGUI() {
            var triggerAction = (TriggerAction) target;
            var index = Array.IndexOf(Values, triggerAction.type);
            var newIndex = EditorGUILayout.Popup("Action Type", index, Names);
            var newType = Values[newIndex];
            if (newIndex != index) {
                Undo.RecordObject(triggerAction, "Change Trigger Action Type");
                triggerAction.type = newType;
            }

            var data = triggerAction.SelectedData ?? AssignDefaultValue(triggerAction, triggerAction.type);
            DrawCheckboxes(triggerAction, data);

            var curType = triggerAction.type;
            if (!Renderers.TryGetValue(curType, out var renderer)) {
                EditorGUILayout.HelpBox("You can't edit this type! Please choose another.", MessageType.Error);
                TriggerAction.CurrentGizmosDrawer = null;
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Data", EditorStyles.boldLabel);
            renderer.DrawGUI(triggerAction);
            TriggerAction.CurrentGizmosDrawer = renderer is ISelectedGizmosDrawer gizmosDrawer ? gizmosDrawer.DrawGizmos : null;
        }

        private static void DrawCheckboxes(TriggerAction triggerAction, BaseTriggerActionData data) {
            if (data == null)
                return;
            var types = ActionManager.TargetTypeValues.Where(v => v != TargetType.None && data.PossibleTargets.HasFlagFast(v)).ToArray();
            if (types.Length < 1)
                return;
            GUILayout.Label("Targets", EditorStyles.boldLabel);
            var value = data.SelectedTargets;
            foreach (var type in types) {
                var active = EditorGUILayout.Toggle(type.ToString(), value.HasFlagFast(type));
                if (active)
                    value |= type;
                else
                    value &= ~type;
            }

            if (value == data.SelectedTargets)
                return;
            Undo.RecordObject(triggerAction, "Change Trigger Action Targets");
            data.SelectedTargets = value;
        }

        private static BaseTriggerActionData AssignDefaultValue(TriggerAction triggerAction, TriggerActionType type) => type switch {
            TriggerActionType.TeleportToPosition => triggerAction.tpToPos ??= new TeleportToPositionData(Vector3.zero),
            TriggerActionType.TeleportToRoom => triggerAction.tpToRoom ??= new TeleportToRoomData(Vector3.zero, "Unknown"),
            TriggerActionType.MoveRelativeToSelf => triggerAction.moveRel ??= new MoveRelativeToSelfData(Vector3.zero),
            TriggerActionType.KillPlayer => triggerAction.killPlayer ??= new KillPlayerData("Killed by your epic trigger."),
            TriggerActionType.TeleportToSpawnedObject => triggerAction.tpToSpawnedObject ??= new RuntimeTeleportToSpawnedObjectData(null, Vector3.zero),
            _ => null
        };

    }

}
