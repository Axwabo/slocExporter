using slocExporter.TriggerActions;

namespace Editor.sloc.TriggerActions.Renderers {

    public interface ITriggerActionEditorRenderer {

        string Description { get; }

        void DrawGUI(TriggerAction instance);

    }

}
