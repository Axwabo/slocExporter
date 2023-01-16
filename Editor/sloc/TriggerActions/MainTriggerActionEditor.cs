using System;
using System.Collections.Generic;
using Editor.sloc.TriggerActions.Renderers;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions {

    [CustomEditor(typeof(TriggerAction))]
    [CanEditMultipleObjects]
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

            if (triggerAction.SelectedData == null) {
                Undo.RecordObject(triggerAction, "Change Trigger Action Type");
                AssignDefaultValue(triggerAction, triggerAction.type);
            }

            var curType = triggerAction.type;
            if (!Renderers.TryGetValue(curType, out var renderer)) {
                EditorGUILayout.HelpBox("You can't edit this type! Please choose another.", MessageType.Error);
                TriggerAction.CurrentGizmosDrawer = null;
                return;
            }

            renderer.DrawGUI(triggerAction);
            TriggerAction.CurrentGizmosDrawer = renderer is ISelectedGizmosDrawer gizmosDrawer ? gizmosDrawer.DrawGizmos : null;
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
                case TriggerActionType.TeleportToSpawnedObject:
                    triggerAction.tpToSpawnedObject ??= new RuntimeTeleportToSpawnedObjectData(null, Vector3.zero);
                    break;
            }
        }

    }

}
