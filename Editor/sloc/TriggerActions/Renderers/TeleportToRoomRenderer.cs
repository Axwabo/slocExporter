using slocExporter.TriggerActions;
using UnityEditor;

namespace Editor.sloc.TriggerActions.Renderers
{

    public sealed class TeleportToRoomRenderer : ITriggerActionEditorRenderer
    {

        public string Description => "Teleports the object to a specified room with the given position offset.";

        public void DrawGUI(TriggerAction instance)
        {
            var data = instance.tpToRoom;
            var room = EditorGUILayout.TextField("Room Name", data.Room);
            if (room != data.Room)
            {
                Undo.RecordObject(instance, "Change Teleport Room");
                data.Room = room;
            }

            SimplePositionRenderer.DrawCommonElements(instance, data);
        }

    }

}
