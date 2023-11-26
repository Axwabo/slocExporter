using System;
using System.Collections.Generic;
using System.Linq;
using Editor.sloc.TriggerActions.Renderers;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using slocExporter.TriggerActions.Enums;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions
{

    [CustomEditor(typeof(TriggerAction))]
    public sealed class MainTriggerActionEditor : UnityEditor.Editor
    {

        private static readonly Dictionary<TriggerActionType, ITriggerActionEditorRenderer> Renderers = new()
        {
            {TriggerActionType.TeleportToPosition, ActionRenderers.TeleportToPosition},
            {TriggerActionType.MoveRelativeToSelf, ActionRenderers.MoveRelativeToSelf},
            {TriggerActionType.TeleportToRoom, ActionRenderers.TeleportToRoom},
            {TriggerActionType.KillPlayer, ActionRenderers.KillPlayer},
            {TriggerActionType.TeleportToSpawnedObject, ActionRenderers.TeleportToSpawnedObject},
            {TriggerActionType.TeleporterImmunity, ActionRenderers.TeleporterImmunity}
        };

        private static readonly TriggerActionType[] Values = (TriggerActionType[]) Enum.GetValues(typeof(TriggerActionType));
        private static readonly string[] Names = Enum.GetNames(typeof(TriggerActionType));

        private static readonly GUIContent TargetsContent = new("Targets", "Types of objects that can trigger this action.");
        private static readonly GUIContent EventsContent = new("Trigger Events", "The trigger event types that this action will listen for.");

        private static readonly Dictionary<TriggerEventType, GUIContent> EventDescriptions = new()
        {
            {TriggerEventType.Enter, new GUIContent("On Enter", "Invoked when an object enters the trigger.")},
            {TriggerEventType.Stay, new GUIContent("On Stay", "Invoked every frame while an object stays in the trigger.")},
            {TriggerEventType.Exit, new GUIContent("On Exit", "Invoked when an object leaves the trigger.")}
        };

        public override void OnInspectorGUI()
        {
            var triggerAction = (TriggerAction) target;
            var index = Array.IndexOf(Values, triggerAction.type);
            var newIndex = EditorGUILayout.Popup("Action Type", index, Names);
            var newType = Values[newIndex];
            if (newIndex != index)
            {
                Undo.RecordObject(triggerAction, "Change Trigger Action Type");
                triggerAction.type = newType;
            }

            var data = triggerAction.SelectedData ?? AssignDefaultValue(triggerAction, triggerAction.type);
            var curType = triggerAction.type;
            if (!Renderers.TryGetValue(curType, out var renderer))
            {
                EditorGUILayout.HelpBox("You can't edit this type! Please choose another one.", MessageType.Error);
                TriggerAction.CurrentGizmosDrawer = null;
                return;
            }

            EditorGUILayout.HelpBox(renderer.Description, MessageType.None);
            DrawCheckboxes(triggerAction, data);
            EditorGUILayout.Space(5);
            triggerAction.ToggleData = EditorGUILayout.Foldout(triggerAction.ToggleData, "Data", true);
            if (triggerAction.ToggleData)
                renderer.DrawGUI(triggerAction);
            TriggerAction.CurrentGizmosDrawer = renderer is ISelectedGizmosDrawer gizmosDrawer ? gizmosDrawer.DrawGizmos : null;
        }

        private static void DrawCheckboxes(TriggerAction triggerAction, BaseTriggerActionData data)
        {
            if (data == null)
                return;
            DrawTargetTypeCheckboxes(triggerAction, data);
            DrawEventTypeCheckboxes(triggerAction, data);
        }

        private static void DrawTargetTypeCheckboxes(TriggerAction triggerAction, BaseTriggerActionData data)
        {
            var types = ActionManager.TargetTypeValues.Where(v => v != TargetType.None && data.PossibleTargets.HasFlagFast(v)).ToArray();
            if (types.Length < 1)
                return;
            EditorGUILayout.Space(5);
            triggerAction.ToggleTargets = EditorGUILayout.Foldout(triggerAction.ToggleTargets, TargetsContent, true);
            if (!triggerAction.ToggleTargets)
                return;
            var value = data.SelectedTargets;
            foreach (var type in types)
            {
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

        private static void DrawEventTypeCheckboxes(TriggerAction triggerAction, BaseTriggerActionData data)
        {
            EditorGUILayout.Space(5);
            triggerAction.ToggleEvents = EditorGUILayout.Foldout(triggerAction.ToggleEvents, EventsContent, true);
            if (!triggerAction.ToggleEvents)
                return;
            var value = data.SelectedEvents;
            foreach (var type in ActionManager.EventTypeValues)
            {
                var content = EventDescriptions.GetValueOrDefault(type) ?? new GUIContent(type.ToString());
                var active = EditorGUILayout.Toggle(content, value.HasFlagFast(type));
                if (active)
                    value |= type;
                else
                    value &= ~type;
            }

            if (value == data.SelectedEvents)
                return;
            Undo.RecordObject(triggerAction, "Change Trigger Action Events");
            data.SelectedEvents = value;
        }

        private static BaseTriggerActionData AssignDefaultValue(TriggerAction triggerAction, TriggerActionType type) => type switch
        {
            TriggerActionType.TeleportToPosition => triggerAction.tpToPos ??= new TeleportToPositionData(Vector3.zero),
            TriggerActionType.TeleportToRoom => triggerAction.tpToRoom ??= new TeleportToRoomData("Unknown", Vector3.zero),
            TriggerActionType.MoveRelativeToSelf => triggerAction.moveRel ??= new MoveRelativeToSelfData(Vector3.zero),
            TriggerActionType.KillPlayer => triggerAction.killPlayer ??= new KillPlayerData("Killed by your epic trigger."),
            TriggerActionType.TeleportToSpawnedObject => triggerAction.tpToSpawnedObject ??= new RuntimeTeleportToSpawnedObjectData(null, Vector3.zero),
            TriggerActionType.TeleporterImmunity => triggerAction.tpImmunity ??= new TeleporterImmunityData(false, ImmunityDurationMode.Absolute, 1),
            _ => null
        };

    }

}
