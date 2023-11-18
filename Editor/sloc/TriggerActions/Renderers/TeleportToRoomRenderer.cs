using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

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

            if (EditorGUILayout.LinkButton("Room names (see attribute values)"))
                Application.OpenURL("https://github.com/Axwabo/SCPSL-Helpers/blob/main/Axwabo.Helpers.NWAPI/Config/RoomType.cs");
            SimplePositionRenderer.DrawCommonElements(instance, data);
        }

    }

}
