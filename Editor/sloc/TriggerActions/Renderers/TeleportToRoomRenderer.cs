using slocExporter.TriggerActions;
using UnityEditor;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class TeleportToRoomRenderer : ITriggerActionEditorRenderer {

        public string Description => "Teleports the object to a specified room with the given position offset.";

        public void DrawGUI(TriggerAction instance) {
            var data = instance.tpToRoom;
            var room = EditorGUILayout.TextField("Room Name", data.Room);
            if (room != data.Room) {
                Undo.RecordObject(instance, "Change Teleport Room");
                data.Room = room;
            }

            var position = EditorGUILayout.Vector3Field("Position Offset", data.Position);
            if (position != data.Position) {
                Undo.RecordObject(instance, "Change Teleport Offset");
                data.Position = position;
            }

            SimplePositionRenderer.DrawCheckboxes(instance, data);
        }

    }

}
