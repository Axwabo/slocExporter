using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEditor;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class SimplePositionRenderer : ITriggerActionEditorRenderer {

        public delegate BaseTeleportData PositionGetter(TriggerAction instance);

        private readonly string _label;
        private readonly PositionGetter _positionGetter;

        public SimplePositionRenderer(string label, PositionGetter positionGetter) {
            _label = label;
            _positionGetter = positionGetter;
        }

        public void DrawGUI(TriggerAction instance) {
            var i = _positionGetter(instance);
            var input = EditorGUILayout.Vector3Field(_label, i.Position);
            if (input != i.Position) {
                Undo.RecordObject(instance, "Change Teleport Offset");
                i.Position = input;
            }

            DrawCheckboxes(instance, i);
        }

        public static void DrawCheckboxes(TriggerAction triggerAction, BaseTeleportData data) {
            var value = data.Options;
            foreach (var type in ActionManager.TeleportOptionsValues) {
                var active = EditorGUILayout.Toggle(ActionManager.TeleportOptionsNames[type], value.HasFlagFast(type));
                if (active)
                    value |= type;
                else
                    value &= ~type;
            }

            if (value == data.Options)
                return;
            EditorUtility.SetDirty(triggerAction);
            Undo.RecordObject(triggerAction, "Change Teleport Options");
            data.Options = value;
        }

    }

}
