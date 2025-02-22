using System;
using System.Collections.Generic;
using System.Linq;
using slocExporter;
using slocExporter.Objects;
using UnityEditor;

namespace Editor.sloc
{

    [CustomEditor(typeof(ColliderModeSetter))]
    [CanEditMultipleObjects]
    [Obsolete]
    public sealed class ColliderModeEditor : UnityEditor.Editor
    {

        private static readonly List<string> Options = Enum.GetValues(typeof(PrimitiveObject.ColliderCreationMode))
            .Cast<PrimitiveObject.ColliderCreationMode>()
            .Select(ColliderModeSetter.ModeToString).ToList();

        private static readonly string[] OptionsArray = Options.ToArray();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is obsolete. Use the PrimitiveFlagsSetter component instead.", MessageType.Warning);
            var targetsCache = targets;
            if (targetsCache.Length < 1)
                return;
            var current = ((ColliderModeSetter) targetsCache[0]).mode;
            var popup = EditorGUILayout.Popup("Collider Creation Mode", Options.IndexOf(ColliderModeSetter.ModeToString(current)), OptionsArray);
            var mode = ColliderModeSetter.StringToMode(OptionsArray[popup]);
            EditorGUILayout.HelpBox(ColliderModeSetter.GetModeDescription(mode), MessageType.Info);
            if (mode == current)
                return;
            Undo.RecordObjects(targetsCache, "Set Collider Mode");
            foreach (var t in targetsCache)
                ((ColliderModeSetter) t).mode = mode;
        }

    }

}
