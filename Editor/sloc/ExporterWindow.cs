using System.IO;
using slocExporter;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.sloc {

    public class ExporterWindow : EditorWindow {

        private const string ProgressbarTitle = "slocExporter";

        [MenuItem("Window/sloc/Export")]
        public static void ShowWindow() => GetWindow(typeof(ExporterWindow), true, "Export to sloc");

        private static string _filePath = @"%appdata%\EXILED\Plugins\sloc\Objects\MyObject";

        private static bool _debug;

        private void OnGUI() {
            GUILayout.Label("File", EditorStyles.boldLabel);
            _filePath = EditorGUILayout.TextField("Path", _filePath);
            if (GUILayout.Button("Select File")) {
                var sceneName = SceneManager.GetActiveScene().name;
                var path = EditorUtility.SaveFilePanel("Save sloc file", Path.GetDirectoryName(_filePath.ToFullAppDataPath()), string.IsNullOrEmpty(sceneName) ? "MyObject" : sceneName, "sloc");
                if (!string.IsNullOrEmpty(path))
                    _filePath = path.ToShortAppDataPath();
            }

            GUILayout.Label("Export", EditorStyles.boldLabel);
            _debug = EditorGUILayout.Toggle("Show Debug", _debug);
            if (GUILayout.Button("Export All"))
                Export(false);
            if (GUILayout.Button("Export Selected"))
                Export(true);
        }

        private static void Export(bool selectedOnly) {
            if (!ObjectExporter.Init(_debug, _filePath, slocAttributes.None)) {
                EditorUtility.DisplayDialog(ProgressbarTitle, "Export is already in progress", "OK");
                return;
            }

            EditorUtility.DisplayProgressBar(ProgressbarTitle, "Starting export", -1f);
            ObjectExporter.TryExport(selectedOnly, ProgressbarUpdate);
            EditorUtility.ClearProgressBar();
        }

        private static void ProgressbarUpdate(string info, float progress) => EditorUtility.DisplayProgressBar(ProgressbarTitle, info, progress);

    }

}
