using slocExporter.TriggerActions;
using UnityEditor;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class TeleportToRoomRenderer : ITriggerActionEditorRenderer {

        public string Description => "Teleports the object to a specified room with the given position offset.";

        public void DrawGUI(TriggerAction instance) {
            var data = instance.tpToRoom;
            data.Room = EditorGUILayout.TextField("Room Name", data.Room);
            data.Position = EditorGUILayout.Vector3Field("Position Offset", data.Position);
            SimplePositionRenderer.DrawCheckboxes(instance, data);
        }

    }

}
