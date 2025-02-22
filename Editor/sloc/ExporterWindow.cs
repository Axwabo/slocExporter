using System.IO;
using slocExporter;
using slocExporter.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.sloc
{

    public sealed class ExporterWindow : EditorWindow
    {

        private const string ProgressbarTitle = "slocExporter";
        private const string LossyColorDescription = "Uses a single 32-bit integer for colors instead of four 32-bit floats (16 bytes per color). This reduces file size but limits the RGB color range to 0-255 and therefore loses precision.";
        private const string Asterisk = "Hover over an item with an * for more information.";
        private const string FilePathStateKey = "slocExporterExportFilePath";

        private static readonly GUIContent FlagsLabel = new("Default Primitive Flags*", "The default flags to use for primitive objects");
        private static readonly GUIContent TriggerActionsLabel = new("Export All Trigger Actions*", "Exports trigger actions for every primitive, even if their flags don't specify it as a trigger.");

        [MenuItem("sloc/Export")]
        public static void ShowWindow() => GetWindow<ExporterWindow>(true, "Export to sloc");

        private static string _filePath = @"%appdata%\EXILED\Plugins\sloc\Objects\MyObject";

        private static bool _debug;

        private static bool _lossyColors;

        private static bool _exportAllTriggerActions;

        private static PrimitiveObjectFlags _flags = PrimitiveObject.DefaultFlags;

        private void OnEnable() => _filePath = SessionState.GetString(FilePathStateKey, _filePath);

        private void OnGUI()
        {
            GUILayout.Label("File", EditorStyles.boldLabel);
            var filePath = _filePath;
            if (GUILayout.Button("Select File"))
            {
                var sceneName = SceneManager.GetActiveScene().name;
                var path = EditorUtility.SaveFilePanel(
                    "Save sloc file", string.IsNullOrEmpty(_filePath) || !Directory.Exists(_filePath.ToFullAppDataPath())
                        ? null
                        : Path.GetDirectoryName(_filePath.ToFullAppDataPath()),
                    string.IsNullOrEmpty(sceneName) ? "MyObject" : sceneName,
                    "sloc"
                );
                if (!string.IsNullOrEmpty(path))
                    filePath = path.ToShortAppDataPath();
            }

            _filePath = EditorGUILayout.TextField("Path", filePath);
            SessionState.SetString(FilePathStateKey, _filePath);
            GUILayout.Space(10);
            GUILayout.Label("Attributes", EditorStyles.boldLabel);
            _lossyColors = EditorGUILayout.Toggle(new GUIContent("Lossy Colors*", LossyColorDescription), _lossyColors);
            _flags = (PrimitiveObjectFlags) EditorGUILayout.EnumFlagsField(FlagsLabel, _flags);
            _exportAllTriggerActions = EditorGUILayout.Toggle(TriggerActionsLabel, _exportAllTriggerActions);
            GUILayout.Space(10);
            GUILayout.Label("Export", EditorStyles.boldLabel);
            _debug = EditorGUILayout.Toggle("Show Debug", _debug);
            if (GUILayout.Button("Export All"))
                Export(false);
            if (GUILayout.Button("Export Selected"))
                Export(true);
            GUILayout.Space(20);
            GUILayout.Label(Asterisk, EditorStyles.centeredGreyMiniLabel);
            if (filePath != _filePath)
                Repaint();
        }

        private static void Export(bool selectedOnly)
        {
            if (!ObjectExporter.Init(_debug, _filePath, CreateAttributes(), PrimitiveObject.ColliderCreationMode.Unset)) // TODO
            {
                EditorUtility.DisplayDialog(ProgressbarTitle, "Export is already in progress", "OK");
                return;
            }

            EditorUtility.DisplayProgressBar(ProgressbarTitle, "Starting export", -1f);
            ObjectExporter.TryExport(selectedOnly, ProgressbarUpdate);
            EditorUtility.ClearProgressBar();
        }

        private static slocAttributes CreateAttributes()
        {
            var attribute = slocAttributes.None;
            if (_lossyColors)
                attribute |= slocAttributes.LossyColors;
            if (_flags != PrimitiveObjectFlags.None)
                attribute |= slocAttributes.DefaultFlags;
            if (_exportAllTriggerActions)
                attribute |= slocAttributes.ExportAllTriggerActions;
            return attribute;
        }

        private static void ProgressbarUpdate(string info, float progress) => EditorUtility.DisplayProgressBar(ProgressbarTitle, info, progress);

    }

}
