using slocExporter;
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
                if (!waypoint.isStatic)
                    continue;
                var t = waypoint.transform;
                if (t.lossyScale == Vector3.one && t.eulerAngles == Vector3.zero)
                    continue;
                EditorGUILayout.HelpBox("Static waypoints aren't affected by scale or rotation.", MessageType.Info);
                break;
            }
        }

    }

}
