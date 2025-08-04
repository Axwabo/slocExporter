using System;
using System.Collections.Generic;
using System.Linq;
using slocExporter;
using slocExporter.Extensions;
using slocExporter.Objects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
            var targetsCache = targets;
            if (targetsCache.Length < 1)
                return;
            if (GUILayout.Button("To PrimitiveFlagsSetter"))
            {
                ConvertToFlags(targetsCache);
                return;
            }

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

        private static void ConvertToFlags(Object[] targetsCache)
        {
            foreach (var o in targetsCache)
            {
                var colliderModeSetter = (ColliderModeSetter) o;
                var flagsSetter = colliderModeSetter.gameObject.AddComponent<PrimitiveFlagsSetter>();
                Undo.RegisterCreatedObjectUndo(flagsSetter, "ColliderModeSetter to PrimitiveFlagsSetter");
                flagsSetter.flags = ColliderModeCompatibility.GetPrimitiveFlags(colliderModeSetter.mode);
                Undo.DestroyObjectImmediate(colliderModeSetter);
            }
        }

    }

}
