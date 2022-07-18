using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using slocExporter;
using slocExporter.Objects;
using UnityEditor;
using UnityEngine;

public static class ObjectExporter {

    public const string ExporterIgnoredTag = "slocExporter Ignored";
    public const string RoomTag = "Room";

    public static readonly Dictionary<Regex, ObjectType> PrimitiveTypes = new Dictionary<string, ObjectType> {
        {"Cube", ObjectType.Cube},
        {"Cylinder", ObjectType.Cylinder},
        {"Sphere", ObjectType.Sphere},
        {"Capsule", ObjectType.Capsule},
        {"Plane", ObjectType.Plane},
        {"Quad", ObjectType.Quad}
    }.ToDictionary(k => new Regex($"{k.Key}(?:(?: Instance)+)?"), v => v.Value);

    public static bool Init(bool debug, string filePath) {
        if (_inProgress)
            return false;
        _debug = debug;
        _fileName = filePath;
        return true;
    }

    private static bool _debug;

    private static string _fileName = @"%appdata%\EXILED\Plugins\sloc\objects\MyObject";

    private static bool _inProgress;

    public static void TryExport(bool selectedOnly, Action<string, float> updateProgress = null) {
        if (_inProgress) {
            EditorUtility.DisplayDialog("slocExporter", "Export is already in progress", "OK");
            return;
        }

        if (string.IsNullOrEmpty(_fileName)) {
            EditorUtility.DisplayDialog("slocExporter", "You must specify a path to export to!", "OK");
            return;
        }

        _inProgress = true;
        try {
            DoExport(selectedOnly, out var exportedCount, updateProgress);
            _inProgress = false;
            EditorUtility.DisplayDialog("slocExporter", $"Export complete.\nsloc created with {exportedCount} GameObject(s).", "OK");
        } catch (Exception e) {
            UnityEngine.Debug.LogError(e);
            _inProgress = false;
            EditorUtility.DisplayDialog("slocExporter", "Export failed. See the Debug log for details.", "OK");
        }
    }

    private static void DoExport(bool selectedOnly, out int exportedCount, Action<string, float> updateProgress = null) {
        var stopwatch = Stopwatch.StartNew();
        var file = (_fileName.EndsWith(".sloc") ? _fileName : $"{_fileName}.sloc").ToFullAppDataPath();
        EnsureDirectoryExists(file);
        LogWarning($"[slocExporter] Starting export to {file}");
        updateProgress?.Invoke("Detecting objects", -1f);
        var allObjects = GetObjects(selectedOnly);
        var objectsById = new Dictionary<int, slocGameObject>();
        var renderers = new Dictionary<int, MeshRenderer>();
        var allObjectsCount = allObjects.Length;
        var floatObjectsCount = (float) allObjectsCount;
        Log($"Found {allObjectsCount} objects in total.");
        for (var i = 0; i < allObjectsCount; i++) {
            var o = allObjects[i];
            var progressString = $"Processing objects ({i + 1} of {allObjectsCount})";
            var progressValue = i / floatObjectsCount;
            if (TaggedAsIgnored(o)) {
                Log($"Skipped object {o.name} because it's tagged as {ExporterIgnoredTag}");
                updateProgress?.Invoke(progressString, progressValue);
                continue;
            }

            foreach (var component in o.GetComponents<Component>()) {
                var skip = component switch {
                    ExporterIgnored _ => IgnoreObject(o, objectsById),
                    MeshFilter meshFilter => ProcessMeshFilter(o, meshFilter, objectsById),
                    MeshRenderer meshRenderer => ProcessRenderer(o, meshRenderer, renderers),
                    Light light => ProcessLight(o, light, objectsById),
                    _ => false
                };
                if (!skip)
                    continue;
                Log($"Skipped object {o.name}");
                break;
            }

            updateProgress?.Invoke(progressString, progressValue);
        }

        Log("Processing material colors...");
        RenderersToMaterials(renderers, objectsById, updateProgress);
        var nonEmpty = objectsById.Where(e => e.Value is {IsEmpty: false}).ToList();
        Log("Writing file...");
        WriteObjects(file, nonEmpty, updateProgress);
        LogWarning($"[slocExporter] Export done in {stopwatch.ElapsedMilliseconds}ms; {nonEmpty.Count} objects exported to {file}");
        exportedCount = nonEmpty.Count;
    }

    private static void EnsureDirectoryExists(string file) {
        if (string.IsNullOrEmpty(file))
            return;
        var dir = Path.GetDirectoryName(file);
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    private static GameObject[] GetObjects(bool selectedOnly) => !selectedOnly ? UnityEngine.Object.FindObjectsOfType<GameObject>() : Selection.gameObjects.SelectMany(e => e.Children()).Distinct().ToArray();

    private static bool TaggedAsIgnored(GameObject gameObject) {
        var root = gameObject.transform.root.gameObject;
        return gameObject.CompareTag(ExporterIgnoredTag) || gameObject.TryGetComponent(out ExporterIgnored _) || root.CompareTag(RoomTag) || root.CompareTag(ExporterIgnoredTag) || root.TryGetComponent(out ExporterIgnored _);
    }

    private static bool IgnoreObject(GameObject gameObject, Dictionary<int, slocGameObject> objectsById) {
        Log($"{gameObject.name} is flagged as ExporterIgnored");
        objectsById.Remove(gameObject.GetInstanceID());
        return true;
    }

    public static void RenderersToMaterials(Dictionary<int, MeshRenderer> renderers, Dictionary<int, slocGameObject> objectList, Action<string, float> updateProgress = null) {
        updateProgress?.Invoke("Setting materials", 0);
        var list = renderers.ToList();
        var count = list.Count;
        var floatCount = (float) count;
        for (var i = 0; i < count; i++) {
            updateProgress?.Invoke($"Setting materials ({i + 1} of {count})", i / floatCount);
            var pair = list[i];
            var r = pair.Value;
            var mat = r.sharedMaterial;
            var id = pair.Key;
            if (mat == null || !objectList.TryGetValue(id, out var obj) || !(obj is PrimitiveObject p))
                continue;
            p.MaterialColor = mat.color;
            Log($"Set material color for {id} to {mat.color}");
        }
    }

    private static void WriteObjects(string file, List<KeyValuePair<int, slocGameObject>> nonEmpty, Action<string, float> updateProgress = null) {
        updateProgress?.Invoke("Writing objects", 0);
        var writer = new BinaryWriter(File.OpenWrite(file), Encoding.UTF8);
        writer.Write(API.slocVersion);
        writer.Write(nonEmpty.Count);
        var count = nonEmpty.Count;
        var floatCount = (float) count;
        for (var i = 0; i < count; i++) {
            var obj = nonEmpty[i];
            obj.Value.WriteTo(writer);
            updateProgress?.Invoke($"Writing objects ({i + 1} of {count})", i / floatCount);
        }

        writer.Close();
    }

    public static void Log(object o) {
        if (_debug)
            UnityEngine.Debug.Log(o);
    }

    public static void LogWarning(object o) {
        if (_debug)
            UnityEngine.Debug.LogWarning(o);
    }

    public static bool ProcessLight(GameObject o, Light l, Dictionary<int, slocGameObject> objectList) {
        Log("Found light " + l.name);
        var oTransform = o.transform;
        objectList.Add(o.GetInstanceID(), new LightObject {
            Transform = {
                Position = oTransform.position,
                Rotation = oTransform.rotation,
                Scale = oTransform.lossyScale
            },
            LightColor = l.color,
            Intensity = l.intensity,
            Range = l.range,
            Shadows = l.shadows != LightShadows.None
        });
        return false;
    }

    public static bool ProcessRenderer(GameObject o, MeshRenderer meshRenderer, Dictionary<int, MeshRenderer> renderers) {
        Log("Found MeshRenderer " + meshRenderer.name);
        renderers.Add(o.GetInstanceID(), meshRenderer);
        return false;
    }

    public static bool ProcessMeshFilter(GameObject o, MeshFilter filter, Dictionary<int, slocGameObject> objectList) {
        var meshName = filter.sharedMesh.name;
        Log($"Found MeshFilter with mesh name {meshName}");
        var type = PrimitiveTypes.FirstOrDefault(e => e.Key.IsMatch(meshName)).Value;
        if (type is ObjectType.None) {
            Log("Mesh does not match any known primitive type, skipping GameObject " + o.name);
            return true;
        }

        Log($"Added PrimitiveObject with type {type}");
        var oTransform = o.transform;
        objectList.Add(o.GetInstanceID(), new PrimitiveObject(type) {
            Transform = {
                Position = oTransform.position,
                Rotation = oTransform.rotation,
                Scale = oTransform.lossyScale
            }
        });
        return false;
    }

}
