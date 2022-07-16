using UnityEditor;
using UnityEngine;

namespace Editor.sloc {

    public class ImporterWindow : EditorWindow {

        [MenuItem("Window/sloc/Import")]
        public static void ShowWindow() => GetWindow(typeof(ImporterWindow), true, "Import an sloc file");

        private static string _filePath = "";

        private void OnGUI() {
            _filePath = EditorGUILayout.TextField("File Path:", _filePath);
            if (GUILayout.Button("Select File"))
                _filePath = EditorUtility.OpenFilePanel("Select sloc file to import", _filePath, "sloc");
            if (!GUILayout.Button("Import Objects"))
                return;
            slocImporter.Init(_filePath);
            slocImporter.TryImport();
        }

    }

}
