using slocExporter;
using UnityEditor;

namespace Editor.sloc
{

    [CustomEditor(typeof(Scp079CameraProperties))]
    public sealed class CameraPropertiesEditor : UnityEditor.Editor
    {

        private const string Message = "Minimum should be less than equal to maximum (per property).\n"
                                       + "Vertical constraint means how much you can look up or down; "
                                       + "-10 means you can look up 10 degrees, 30 means you can look down 30 degrees.\n"
                                       + "Horizontal constraint means how much you can look left or right; "
                                       + "-10 means you can look left 10 degrees, 30 means you can look right 30 degrees.\n"
                                       + "Zoom constraint means how much you can zoom in or out; "
                                       + "0 is 0%, 1 is 100%.\n"
                                       + "Gizmos indicate the rotation constraints.";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox(Message, MessageType.Info);
        }

    }

}
