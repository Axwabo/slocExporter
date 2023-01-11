using System;
using System.Collections.Generic;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions {

    [CustomEditor(typeof(TriggerAction))]
    [CanEditMultipleObjects]
    public sealed class MainTriggerActionEditor : UnityEditor.Editor {

        private static readonly Dictionary<TriggerActionType, TriggerActionEditorRenderer> Renderers = new() {
            {TriggerActionType.TeleportToPosition, StandardActionRenderers.TeleportToPosition},
            {TriggerActionType.MoveRelativeToSelf, StandardActionRenderers.MoveRelativeToSelf},
            {TriggerActionType.TeleportToRoom, StandardActionRenderers.TeleportToRoom},
            {TriggerActionType.KillPlayer, StandardActionRenderers.KillPlayer},
            {TriggerActionType.TeleportToSpawnedObject, TeleportToSpawnedObjectRenderer.Draw},
        };

        private static readonly TriggerActionType[] Values = (TriggerActionType[]) Enum.GetValues(typeof(TriggerActionType));
        private static readonly string[] Names = Enum.GetNames(typeof(TriggerActionType));

        public override void OnInspectorGUI() {
            var triggerAction = (TriggerAction) target;

            var index = Array.IndexOf(Values, triggerAction.type);
            var newIndex = EditorGUILayout.Popup("Action Type", index, Names);
            var newType = Values[newIndex];
            if (newIndex != index) {
                triggerAction.type = newType;
                AssignDefaultValue(triggerAction, newType);
            }

            var curType = triggerAction.type;

            if (Renderers.TryGetValue(curType, out var renderer))
                renderer(triggerAction);
            else
                EditorGUILayout.HelpBox("You can't edit this type! Please choose another.", MessageType.Error);
        }

        private static void AssignDefaultValue(TriggerAction triggerAction, TriggerActionType type) {
            switch (type) {
                case TriggerActionType.TeleportToPosition:
                    triggerAction.tpToPos ??= new TeleportToPositionData(Vector3.zero);
                    break;
                case TriggerActionType.TeleportToRoom:
                    triggerAction.tpToRoom ??= new TeleportToRoomData(Vector3.zero, "Unknown");
                    break;
                case TriggerActionType.MoveRelativeToSelf:
                    triggerAction.moveRel ??= new MoveRelativeToSelfData(Vector3.zero);
                    break;
                case TriggerActionType.KillPlayer:
                    triggerAction.killPlayer ??= new KillPlayerData("Killed by your epic trigger.");
                    break;
            }
        }

    }

}
