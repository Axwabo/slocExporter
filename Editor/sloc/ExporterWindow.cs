using System.IO;
using System.Runtime.InteropServices;
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

        private static bool _lossyColor;

        private void OnGUI() {
            GUILayout.Label("File", EditorStyles.boldLabel);
            _filePath = EditorGUILayout.TextField("Path", _filePath);
            if (GUILayout.Button("Select File")) {
                var sceneName = SceneManager.GetActiveScene().name;
                var path = EditorUtility.SaveFilePanel("Save sloc file", Path.GetDirectoryName(_filePath.ToFullAppDataPath()), string.IsNullOrEmpty(sceneName) ? "MyObject" : sceneName, "sloc");
                if (!string.IsNullOrEmpty(path))
                    _filePath = path.ToShortAppDataPath();
            }

            GUILayout.Label("Attributes", EditorStyles.boldLabel);
            _lossyColor = EditorGUILayout.Toggle(new GUIContent("Lossy Colors", "Uses a single 8-bit integer for colors instead of 4 8-bit floats. This reduces file size but limits the RGB color range to 0-255 and therefore loses precision."), _lossyColor);
            GUILayout.Label("Export", EditorStyles.boldLabel);
            _debug = EditorGUILayout.Toggle("Show Debug", _debug);
            if (GUILayout.Button("Export All"))
                Export(false);
            if (GUILayout.Button("Export Selected"))
                Export(true);
        }

        private static void Export(bool selectedOnly) {
            if (!ObjectExporter.Init(_debug, _filePath, CreateAttributes())) {
                EditorUtility.DisplayDialog(ProgressbarTitle, "Export is already in progress", "OK");
                return;
            }

            EditorUtility.DisplayProgressBar(ProgressbarTitle, "Starting export", -1f);
            ObjectExporter.TryExport(selectedOnly, ProgressbarUpdate);
            EditorUtility.ClearProgressBar();
        }

        private static slocAttributes CreateAttributes() {
            var attribute = slocAttributes.None;
            if (_lossyColor)
                attribute |= slocAttributes.LossyColors;
            return attribute;
        }

        private static void ProgressbarUpdate(string info, float progress) => EditorUtility.DisplayProgressBar(ProgressbarTitle, info, progress);

    }

}
