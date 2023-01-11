using slocExporter.TriggerActions;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc.TriggerActions.Renderers {

    public sealed class SimplePositionRenderer : ITriggerActionEditorRenderer {

        public delegate Vector3 PositionGetter(TriggerAction instance);

        public delegate void PositionSetter(TriggerAction instance, Vector3 value);

        private readonly string _label;
        private readonly PositionGetter _positionGetter;
        private readonly PositionSetter _positionSetter;

        public SimplePositionRenderer(string label, PositionGetter positionGetter, PositionSetter positionSetter) {
            _label = label;
            _positionGetter = positionGetter;
            _positionSetter = positionSetter;
        }

        public void DrawGUI(TriggerAction instance) =>
            _positionSetter(instance, EditorGUILayout.Vector3Field(_label, _positionGetter(instance)));

    }

}
