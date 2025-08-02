using slocExporter;
using slocExporter.Serialization.Exporting.Identifiers;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc
{

    [CustomEditor(typeof(Waypoint))]
    [CanEditMultipleObjects]
    public sealed class WaypointEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            foreach (var o in targets)
            {
                var waypoint = (Waypoint) o;
                if (!waypoint.isStatic || waypoint.transform.lossyScale == Vector3.one)
                    continue;
                EditorGUILayout.HelpBox(WaypointIdentifier.Warning, MessageType.Warning);
                break;
            }
        }

    }

}
