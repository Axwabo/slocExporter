using slocExporter;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc
{

    public sealed class ImporterWindow : EditorWindow
    {

        private const string ProgressbarTitle = "slocImporter";
        private const string FilePathStateKey = "slocExporterImportFilePath";

        [MenuItem("sloc/Import")]
        public static void ShowWindow() => GetWindow<ImporterWindow>(true, "Import an sloc file");

        private static string _filePath = "";

        private static bool _useExistingMaterials = true;

        private static bool _colorsFolderOnly;

        private void OnEnable() => SessionState.GetString(FilePathStateKey, _filePath);

        private void OnGUI()
        {
            _useExistingMaterials = EditorGUILayout.Toggle(new GUIContent("Use Existing Materials", "When enabled, the importer tries to use existing materials in the assets."), _useExistingMaterials);
            _colorsFolderOnly = EditorGUILayout.Toggle(new GUIContent("Search in Colors", "When enabled, only materials in the Assets/Colors folder will be attempted to be used."), _colorsFolderOnly);
            _filePath = EditorGUILayout.TextField("File Path:", _filePath);
            if (GUILayout.Button("Select File"))
                _filePath = EditorUtility.OpenFilePanel("Select sloc file to import", _filePath, "sloc").ToShortAppDataPath();
            SessionState.SetString(FilePathStateKey, _filePath);
            if (!GUILayout.Button("Import Objects"))
                return;
            if (!slocImporter.Init(_filePath, _useExistingMaterials, _colorsFolderOnly))
            {
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
