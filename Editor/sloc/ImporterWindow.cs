using slocExporter;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc {

    public class ImporterWindow : EditorWindow {

        private const string ProgressbarTitle = "slocImporter";

        [MenuItem("Window/sloc/Import")]
        public static void ShowWindow() => GetWindow(typeof(ImporterWindow), true, "Import an sloc file");

        private static string _filePath = "";

        private void OnGUI() {
            _filePath = EditorGUILayout.TextField("File Path:", _filePath);
            if (GUILayout.Button("Select File"))
                _filePath = EditorUtility.OpenFilePanel("Select sloc file to import", _filePath, "sloc").ToShortAppDataPath();
            if (!GUILayout.Button("Import Objects"))
                return;
            if (!slocImporter.Init(_filePath)) {
                EditorUtility.DisplayDialog(ProgressbarTitle, "Import is already in progress", "OK");
                return;
            }

            EditorUtility.DisplayProgressBar(ProgressbarTitle, "Starting import", -1f);
            slocImporter.TryImport(ProgressbarUpdate);
            EditorUtility.ClearProgressBar();
        }

        private static void ProgressbarUpdate(string info, float progress) => EditorUtility.DisplayProgressBar(ProgressbarTitle, info, progress);

    }

}
