﻿using System;
using System.Collections.Generic;
using System.Linq;
using slocExporter;
using slocExporter.Objects;
using UnityEditor;
using UnityEngine;
using static slocExporter.ColliderModeSetter;

namespace Editor.sloc {

    [CustomEditor(typeof(ColliderModeSetter))]
    [CanEditMultipleObjects]
    public sealed class ColliderModeEditor : UnityEditor.Editor {

        private static readonly List<string> Options = Enum.GetValues(typeof(PrimitiveObject.ColliderCreationMode))
            .Cast<PrimitiveObject.ColliderCreationMode>()
            .Select(ModeToString).ToList();

        private static readonly string[] OptionsArray = Options.ToArray();

        public override void OnInspectorGUI() {
            var targetsCache = targets;
            if (targetsCache.Length < 1)
                return;
            var current = ((ColliderModeSetter) targetsCache[0]).mode;
            var mode = StringToMode(OptionsArray[EditorGUILayout.Popup("Collider Creation Mode", Options.IndexOf(ModeToString(current)), OptionsArray)]);
            EditorGUILayout.HelpBox(GetModeDescription(mode), MessageType.Info);
            foreach (var t in targetsCache)
                ((ColliderModeSetter) t).mode = mode;
        }

    }

}
