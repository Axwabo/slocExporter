using slocExporter.TriggerActions;
using UnityEditor;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class TeleportToRoomRenderer : ITriggerActionEditorRenderer {

        public void DrawGUI(TriggerAction instance) {
            var data = instance.tpToRoom;
            data.room = EditorGUILayout.TextField("Room Name", data.room);
            data.positionOffset = EditorGUILayout.Vector3Field("Position Offset", data.positionOffset);
        }

    }

}
