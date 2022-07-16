using System;
using System.IO;
using slocExporter;
using UnityEditor;
using UnityEngine;

public static class slocImporter {

    public static void Init(string filePath) {
        if (!_inProgress)
            _filePath = filePath;
    }

    private static string _filePath = "";

    private static bool _inProgress;

    public static void TryImport() {
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
            DoImport(out var importedCount, out var objectName);
            EditorUtility.DisplayDialog("Import complete", $"Imported {importedCount} GameObjects as {objectName}", "OK");
        } catch (Exception e) {
            Debug.LogError(e);
            _inProgress = false;
            EditorUtility.DisplayDialog("slocImporter", "Import failed. See the debug log for details.", "OK");
        }
    }

    private static void DoImport(out int importedCount, out string objectName) {
        EnsureDirectory();
        API.SkipForAll = false;
        API.CreateForAll = false;
        var lastView = SceneView.lastActiveSceneView;
        var cam = lastView ? lastView.camera : null;
        var camTransform = cam ? cam.transform : null;
        var parent = API.CreateObjectsFromFile(_filePath, out var spawned, camTransform ? camTransform.position + camTransform.forward * 3f : Vector3.zero);
        objectName = Path.GetFileNameWithoutExtension(_filePath);
        parent.name = $"Imported-{objectName}";
        _inProgress = false;
        importedCount = spawned;
    }

    private static void EnsureDirectory() {
        if (!Directory.Exists("Assets/Colors"))
            Directory.CreateDirectory("Assets/Colors");
    }

}
