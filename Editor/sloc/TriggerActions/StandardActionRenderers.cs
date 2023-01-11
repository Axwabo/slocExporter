using slocExporter.TriggerActions;
using UnityEditor;

namespace Editor.sloc.TriggerActions {

    public static class StandardActionRenderers {

        public static void TeleportToPosition(TriggerAction instance) =>
            instance.tpToPos.position = EditorGUILayout.Vector3Field("Absolute Position", instance.tpToPos.position);

        public static void TeleportToRoom(TriggerAction instance) {
            var data = instance.tpToRoom;
            data.room = EditorGUILayout.TextField("Room", data.room);
            data.positionOffset = EditorGUILayout.Vector3Field("Position Offset", data.positionOffset);
        }

        public static void KillPlayer(TriggerAction instance) =>
            instance.killPlayer.cause = EditorGUILayout.TextField("Cause", instance.killPlayer.cause);

        public static void MoveRelativeToSelf(TriggerAction instance) =>
            instance.moveRel.position = EditorGUILayout.Vector3Field("Relative Position", instance.moveRel.position);

    }

}
