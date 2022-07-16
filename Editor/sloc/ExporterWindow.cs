using System.IO;
using slocExporter;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.sloc {

    public class ExporterWindow : EditorWindow {

        [MenuItem("Window/sloc/Export")]
        public static void ShowWindow() => GetWindow(typeof(ExporterWindow), true, "Export to sloc");

        internal static string FilePath = @"%appdata%\EXILED\Plugins\sloc\Objects\MyObject";

        internal static bool Debug;

        private void OnGUI() {
            GUILayout.Label("File", EditorStyles.boldLabel);
            FilePath = EditorGUILayout.TextField("Path", FilePath);
            if (GUILayout.Button("Select File")) {
                var sceneName = SceneManager.GetActiveScene().name;
                var path = EditorUtility.SaveFilePanel("Save sloc file", Path.GetDirectoryName(FilePath.ToFullAppDataPath()), string.IsNullOrEmpty(sceneName) ? "MyObject" : sceneName, "sloc");
                if (!string.IsNullOrEmpty(path))
                    FilePath = path;
            }

            GUILayout.Label("Export", EditorStyles.boldLabel);
            Debug = EditorGUILayout.Toggle("Show Debug", Debug);
            if (GUILayout.Button("Export All")) {
                ObjectExporter.Init(Debug, FilePath);
                ObjectExporter.TryExport(false);
            }

            if (GUILayout.Button("Export Selected")) {
                ObjectExporter.Init(Debug, FilePath);
                ObjectExporter.TryExport(true);
            }
        }

    }

}
