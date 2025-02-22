using System.IO;
using slocExporter;
using slocExporter.Objects;
using slocExporter.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.sloc
{

    public sealed class ExporterWindow : EditorWindow
    {

        private const string ProgressbarTitle = "slocExporter";
        private const string FilePathStateKey = "slocExporterExportFilePath";

        [MenuItem("sloc/Export")]
        public static void ShowWindow() => GetWindow<ExporterWindow>(true, "Export to sloc");

        private static string _filePath = @"%appdata%\EXILED\Plugins\sloc\Objects\MyObject";

        private static bool _debug;

        private static ExportPreset _settings;

        private static SerializedObject _settingsSerialized;

        private static ExportPreset _selectedPreset;

        private void OnEnable()
        {
            _filePath = SessionState.GetString(FilePathStateKey, _filePath);
            if (!_settings)
                _settings = CreateInstance<ExportPreset>();
            _settingsSerialized?.Dispose();
            _settingsSerialized = new SerializedObject(_settings);
        }

        private void OnDestroy()
        {
            _settingsSerialized.Dispose();
            _settingsSerialized = null;
        }

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
            _selectedPreset = EditorGUILayout.ObjectField("Preset", _selectedPreset, typeof(ExportPreset), false) as ExportPreset;
            if (!_selectedPreset)
                DrawDefaultSettingsEditor();
            else if (GUILayout.Button("Copy Preset"))
                CopyPreset();
            GUILayout.Space(10);
            GUILayout.Label("Export", EditorStyles.boldLabel);
            _debug = EditorGUILayout.Toggle("Show Debug", _debug);
            if (GUILayout.Button("Export All"))
                Export(false);
            if (GUILayout.Button("Export Selected"))
                Export(true);
            GUILayout.Space(20);
            if (filePath != _filePath)
                Repaint();
        }

        private static void DrawDefaultSettingsEditor()
        {
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            _settingsSerialized.UpdateIfRequiredOrScript();
            var iterator = _settingsSerialized.GetIterator();
            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                if (iterator.propertyPath != "m_Script")
                    EditorGUILayout.PropertyField(iterator, true);
            _settingsSerialized.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
            if (!GUILayout.Button("Save Preset"))
                return;
            var path = EditorUtility.SaveFilePanelInProject("Save Preset", ExportPreset.DefaultName, "asset", "Save preset");
            if (string.IsNullOrEmpty(path))
                return;
            _selectedPreset = Instantiate(_settings);
            AssetDatabase.CreateAsset(_selectedPreset, path);
            AssetDatabase.SaveAssets();
        }

        private static void CopyPreset()
        {
            _settings.lossyColors = _selectedPreset.lossyColors;
            _settings.defaultPrimitiveFlags = _selectedPreset.defaultPrimitiveFlags;
            _settings.exportAllTriggerActions = _selectedPreset.exportAllTriggerActions;
            _selectedPreset = null;
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
            var preset = _selectedPreset ? _selectedPreset : _settings;
            var attribute = slocAttributes.None;
            if (preset.lossyColors)
                attribute |= slocAttributes.LossyColors;
            if (preset.defaultPrimitiveFlags != PrimitiveObjectFlags.None)
                attribute |= slocAttributes.DefaultFlags;
            if (preset.exportAllTriggerActions)
                attribute |= slocAttributes.ExportAllTriggerActions;
            return attribute;
        }

        private static void ProgressbarUpdate(string info, float progress) => EditorUtility.DisplayProgressBar(ProgressbarTitle, info, progress);

    }

}
