using System;
using System.IO;
using slocExporter;
using UnityEditor;
using UnityEngine;

public static class slocImporter {

    public static bool Init(string filePath) {
        if (_inProgress)
            return false;
        _filePath = filePath;
        return true;
    }

    private static string _filePath = "";

    private static bool _inProgress;

    public static void TryImport(Action<string, float> updateProgress = null) {
        if (_inProgress) {
            EditorUtility.DisplayDialog("slocImporter", "Import is already in progress", "OK");
            return;
        }

        if (string.IsNullOrEmpty(_filePath)) {
            EditorUtility.DisplayDialog("slocImporter", "You must specify a file to import!", "OK");
            return;
        }

        _inProgress = true;

        try {
            DoImport(out var importedCount, out var objectName, updateProgress);
            EditorUtility.DisplayDialog("Import complete", $"Imported {importedCount} GameObject(s) as {objectName}", "OK");
        } catch (Exception e) {
            Debug.LogError(e);
            _inProgress = false;
            EditorUtility.DisplayDialog("slocImporter", "Import failed. See the debug log for details.", "OK");
        }
    }

    private static void DoImport(out int importedCount, out string objectName, Action<string, float> updateProgress = null) {
        EnsureDirectory();
        API.SkipForAll = false;
        API.CreateForAll = false;
        var lastView = SceneView.lastActiveSceneView;
        var cam = lastView ? lastView.camera : null;
        var camTransform = cam ? cam.transform : null;
        var fullPath = _filePath.ToFullAppDataPath();
        var parent = API.CreateObjectsFromFile(fullPath, out var spawned, camTransform ? camTransform.position + camTransform.forward * 3f : Vector3.zero, updateProgress: updateProgress);
        objectName = Path.GetFileNameWithoutExtension(fullPath);
        parent.name = $"Imported-{objectName}";
        _inProgress = false;
        importedCount = spawned;
    }

    private static void EnsureDirectory() {
        if (!Directory.Exists("Assets/Colors"))
            Directory.CreateDirectory("Assets/Colors");
    }

}
