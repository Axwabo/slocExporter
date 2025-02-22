using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using slocExporter;
using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.Readers;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

[Obsolete("Use FileExporter instead.")]
public static class ObjectExporter
{

    #region Primitives and Constants

    public const string ExporterIgnoredTag = Identify.ExporterIgnoredTag;
    public const string RoomTag = Identify.RoomTag;

    public static readonly Dictionary<Regex, ObjectType> PrimitiveTypes = Identify.PrimitiveTypes;

    public static ObjectType FindObjectType(string meshName) => Identify.PrimitiveObjectType(meshName);

    public static readonly Dictionary<string, StructureObject.StructureType> StructureGuids = Identify.StructureGuids;

    #endregion

    #region Settings

    private static bool _debug;

    private static string _fileName = @"%appdata%\EXILED\Plugins\sloc\objects\MyObject";

    private static slocAttributes _attributes;

    private static PrimitiveObject.ColliderCreationMode _colliderCreationMode = PrimitiveObject.ColliderCreationMode.Unset;

    private static bool _inProgress;

    public static bool Init(bool debug, string filePath, slocAttributes attributes, PrimitiveObject.ColliderCreationMode colliderCreationMode)
    {
        if (_inProgress)
            return false;
        _debug = debug;
        _fileName = filePath;
        _attributes = attributes;
        _colliderCreationMode = colliderCreationMode;
        return true;
    }

    #endregion

    #region Main Export Process

    public static void TryExport(bool selectedOnly, ProgressUpdater updateProgress = null)
    {
        if (_inProgress)
        {
            EditorUtility.DisplayDialog("slocExporter", "Export is already in progress", "OK");
            return;
        }

        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("slocExporter", "You must specify a path to export to!", "OK");
            return;
        }

        _inProgress = true;
        try
        {
            DoExport(selectedOnly, out var exportedCount, updateProgress);
            EditorUtility.DisplayDialog("slocExporter", $"Export complete.\nsloc created with {exportedCount} GameObject(s).", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            EditorUtility.DisplayDialog("slocExporter", "Export failed. See the Debug log for details.", "OK");
        }
        finally
        {
            _inProgress = false;
        }
    }

    private static void DoExport(bool selectedOnly, out int exportedCount, ProgressUpdater updateProgress = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var file = (_fileName.EndsWith(".sloc") ? _fileName : $"{_fileName}.sloc").ToFullAppDataPath();
        var attributes = _attributes;
        var defaultMode = _colliderCreationMode;
        EnsureDirectoryExists(file);
        LogWarning($"[slocExporter] Starting export to {file.ToShortAppDataPath()}");
        updateProgress?.Invoke("Detecting objects", -1f);
        var allObjects = GetObjects(selectedOnly);
        var objectsById = new InstanceDictionary<slocGameObject>();
        var renderers = new InstanceDictionary<MeshRenderer>();
        var colliders = new InstanceDictionary<PrimitiveObject.ColliderCreationMode>();
        var runtimeTriggerActions = new InstanceDictionary<List<BaseTriggerActionData>>();
        var allObjectsCount = allObjects.Length;
        var floatObjectsCount = (float) allObjectsCount;
        Log($"Found {allObjectsCount} objects in total.");
        for (var i = 0; i < allObjectsCount; i++)
        {
            var o = allObjects[i];
            var progressString = $"Processing objects ({i + 1} of {allObjectsCount})";
            var progressValue = i / floatObjectsCount;
            if (IsTaggedAsIgnored(o))
            {
                Log($"Skipped object {o.name} because it's tagged as {ExporterIgnoredTag}");
                updateProgress?.Invoke(progressString, progressValue);
                continue;
            }

            if (!ProcessGameObject(o, objectsById, defaultMode, colliders, runtimeTriggerActions, renderers))
                CheckIfEmpty(o, objectsById);
            updateProgress?.Invoke(progressString, progressValue);
        }

        Log("Processing material colors...");
        RenderersToMaterials(renderers, objectsById, updateProgress);
        var nonEmpty = new InstanceList(objectsById.Where(e => e.Value is {IsValid: true}));
        Log("Setting collider modes...");
        SetColliderModes(nonEmpty, colliders, updateProgress);
        Log("Setting trigger actions...");
        SetTriggerActions(nonEmpty, runtimeTriggerActions, updateProgress);
        Log("Writing file...");
        WriteObjects(file, nonEmpty, attributes, defaultMode, updateProgress);
        LogWarning($"[slocExporter] Export done in {stopwatch.ElapsedMilliseconds}ms; {nonEmpty.Count} objects exported to {file.ToShortAppDataPath()}");
        exportedCount = nonEmpty.Count;
    }

    private static bool ProcessGameObject(GameObject o, InstanceDictionary<slocGameObject> objectsById, PrimitiveObject.ColliderCreationMode defaultMode, InstanceDictionary<PrimitiveObject.ColliderCreationMode> colliders, InstanceDictionary<List<BaseTriggerActionData>> runtimeTriggerActions, InstanceDictionary<MeshRenderer> renderers)
    {
        if (PrefabUtility.IsPartOfAnyPrefab(o))
        {
            ProcessPrefab(o, objectsById);
            return true;
        }

        foreach (var component in o.GetComponents<Component>())
        {
            var skip = component switch
            {
                StructureOverride structureOverride => ProcessStructureOverride(o, structureOverride, objectsById),
                Collider collider => ProcessCollider(defaultMode, o, collider, colliders),
                ColliderModeSetter setter => SetMode(o, setter, colliders),
                TriggerAction triggerAction => AddTriggerAction(o, triggerAction, runtimeTriggerActions),
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

        return false;
    }

    private static void ProcessPrefab(GameObject gameObject, InstanceDictionary<slocGameObject> objectsById)
    {
        if (!PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject))
            return;
        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
        var guid = prefab == null ? "" : AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(prefab)).ToString();
        var hasOverride = gameObject.TryGetComponent(out StructureOverride structureOverride);
        var type = hasOverride ? structureOverride.type : StructureObject.StructureType.None;
        if (type == StructureObject.StructureType.None && !StructureGuids.TryGetValue(guid, out type))
            return;
        var id = gameObject.GetInstanceID();
        var parent = gameObject.transform.parent;
        objectsById[id] = new StructureObject(id, type)
        {
            ParentId = parent == null ? id : parent.gameObject.GetInstanceID(),
            Transform = gameObject.transform,
            RemoveDefaultLoot = hasOverride && structureOverride.removeDefaultLoot
        };
    }

    private static bool ProcessStructureOverride(GameObject gameObject, StructureOverride structureOverride, InstanceDictionary<slocGameObject> objectsById, StructureObject.StructureType fromPrefab = StructureObject.StructureType.None)
    {
        var type = fromPrefab != StructureObject.StructureType.None ? fromPrefab : structureOverride.type;
        if (type == StructureObject.StructureType.None)
            return false;
        var id = gameObject.GetInstanceID();
        var parent = gameObject.transform.parent;
        objectsById[id] = new StructureObject(id, type)
        {
            ParentId = parent == null ? id : parent.gameObject.GetInstanceID(),
            Transform = gameObject.transform,
            RemoveDefaultLoot = structureOverride.removeDefaultLoot
        };
        return false;
    }

    private static void EnsureDirectoryExists(string file)
    {
        if (string.IsNullOrEmpty(file))
            return;
        var dir = Path.GetDirectoryName(file);
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    private static void WriteObjects(string file, InstanceList nonEmpty, slocAttributes attributes, PrimitiveObject.ColliderCreationMode colliderMode, ProgressUpdater updateProgress = null)
    {
        updateProgress?.Invoke("Writing objects", 0);
        var writer = new BinaryWriter(File.Open(file, FileMode.Create), Encoding.UTF8);
        writer.Write(API.slocVersion);
        var count = nonEmpty.Count;
        var header = new slocHeader(API.slocVersion, count, attributes, colliderMode);
        header.WriteTo(writer);
        var floatCount = (float) count;
        for (var i = 0; i < count; i++)
        {
            nonEmpty.ObjectAt(i).WriteTo(writer, header);
            updateProgress?.Invoke($"Writing objects ({i + 1} of {count})", i / floatCount);
        }

        writer.Close();
    }

    #endregion

    #region Basic Methods

    private static void Log(object o)
    {
        if (_debug)
            Debug.Log(o);
    }

    private static void LogWarning(object o)
    {
        if (_debug)
            Debug.LogWarning(o);
    }

    private static GameObject[] GetObjects(bool selectedOnly)
    {
        var list = new List<GameObject>();
        if (!selectedOnly)
        {
            foreach (var obj in Object.FindObjectsOfType<GameObject>())
            {
                if (obj.transform.parent != null)
                    continue;
                list.AddRange(obj.WithAllChildren());
            }

            return list.ToArray();
        }

        foreach (var o in Selection.gameObjects)
            if (!list.Contains(o))
                list.AddRange(o.WithAllChildren());
        return list.ToArray();
    }

    private static void CheckIfEmpty(GameObject o, InstanceDictionary<slocGameObject> objectList)
    {
        var id = o.GetInstanceID();
        if (objectList.ContainsKey(id))
            return;
        var oTransform = o.transform;
        var parent = oTransform.parent;
        if (oTransform.childCount > 0)
            objectList[id] = new EmptyObject(id)
            {
                ParentId = parent == null ? id : parent.gameObject.GetInstanceID(),
                Transform = oTransform
            };
    }

    private static bool IsTaggedAsIgnored(GameObject gameObject)
    {
        var root = gameObject.transform.root.gameObject;
        return gameObject.tag is ExporterIgnoredTag || gameObject.TryGetComponent(out ExporterIgnored _) || root.tag is RoomTag || root.tag is ExporterIgnoredTag || root.TryGetComponent(out ExporterIgnored _);
    }

    #endregion

    #region Behavior Processors

    private static bool AddTriggerAction(GameObject o, TriggerAction action, InstanceDictionary<List<BaseTriggerActionData>> runtimeTriggerActions)
    {
        var selected = action.SelectedData;
        if (selected == null)
            return false;
        var id = o.GetInstanceID();
        runtimeTriggerActions.GetOrAdd(id, () => new List<BaseTriggerActionData>()).Add(selected);
        return false;
    }

    private static bool ProcessCollider(PrimitiveObject.ColliderCreationMode defaultMode, GameObject o, Collider collider, InstanceDictionary<PrimitiveObject.ColliderCreationMode> modes)
    {
        if (defaultMode is not PrimitiveObject.ColliderCreationMode.Unset)
            return false;
        var id = o.GetInstanceID();
        var colliderMode = collider.isTrigger ? PrimitiveObject.ColliderCreationMode.Trigger : PrimitiveObject.ColliderCreationMode.Unset;
        modes[id] = API.CombineSafe(modes.TryGetValue(id, out var mode) ? mode : PrimitiveObject.ColliderCreationMode.Unset, colliderMode);
        return false;
    }

    private static bool ProcessLight(GameObject o, Light l, InstanceDictionary<slocGameObject> objectList)
    {
        Log("Found light " + l.name);
        var oTransform = o.transform;
        var id = o.GetInstanceID();
        var parent = oTransform.parent;
        objectList.Add(id, new LightObject(id)
        {
            ParentId = parent == null ? id : parent.gameObject.GetInstanceID(),
            Transform = oTransform,
            LightColor = l.color,
            Intensity = l.intensity,
            Range = l.range,
            Shadows = l.shadows != LightShadows.None
        });
        return false;
    }

    private static bool ProcessRenderer(GameObject o, MeshRenderer meshRenderer, InstanceDictionary<MeshRenderer> renderers)
    {
        Log("Found MeshRenderer " + meshRenderer.name);
        renderers[o.GetInstanceID()] = meshRenderer;
        return false;
    }

    private static bool ProcessMeshFilter(GameObject o, MeshFilter filter, InstanceDictionary<slocGameObject> objectList)
    {
        var mesh = filter.sharedMesh;
        if (mesh == null)
        {
            LogWarning($"{o.name} has no mesh");
            return true;
        }

        var meshName = mesh.name;
        Log($"Found MeshFilter with mesh name {meshName}");
        var type = FindObjectType(meshName);
        if (type == ObjectType.None)
        {
            Log("Mesh does not match any known primitive type, skipping GameObject " + o.name);
            return true;
        }

        Log($"Added PrimitiveObject with type {type}");
        var t = o.transform;
        var id = o.GetInstanceID();
        var parent = t.parent;
        objectList[id] = new PrimitiveObject(id, type)
        {
            ParentId = parent == null ? id : parent.gameObject.GetInstanceID(),
            Transform = t,
            ColliderMode = PrimitiveObject.ColliderCreationMode.Unset
        };
        return false;
    }

    private static bool SetMode(GameObject gameObject, ColliderModeSetter setter, InstanceDictionary<PrimitiveObject.ColliderCreationMode> colliderCreationModes)
    {
        var mode = setter.mode;
        if (mode is PrimitiveObject.ColliderCreationMode.Unset)
            return false;
        var id = gameObject.GetInstanceID();
        Log("Setting collider mode for " + id + " to " + mode);
        colliderCreationModes[id] = mode;
        return false;
    }

    #endregion

    #region Post-Processing

    private static void RenderersToMaterials(InstanceDictionary<MeshRenderer> renderers, InstanceDictionary<slocGameObject> objectList, ProgressUpdater updateProgress = null)
    {
        updateProgress?.Invoke("Setting materials", 0);
        var list = renderers.ToList();
        var count = list.Count;
        var floatCount = (float) count;
        for (var i = 0; i < count; i++)
        {
            updateProgress?.Invoke($"Setting materials ({i + 1} of {count})", i / floatCount);
            var (id, r) = list[i];
            var mat = r.sharedMaterial;
            if (mat == null || !objectList.TryGet(id, out PrimitiveObject p))
                continue;
            p.MaterialColor = mat.color;
            Log($"Set material color for {id} to {mat.color}");
        }
    }

    private static void SetColliderModes(InstanceList objects, InstanceDictionary<PrimitiveObject.ColliderCreationMode> modes, ProgressUpdater updateProgress)
    {
        var count = objects.Count;
        var floatCount = (float) count;
        for (var i = 0; i < count; i++)
        {
            updateProgress?.Invoke($"Setting collider modes ({i + 1} of {count})", i / floatCount);
            if (!objects.TryGet(i, out var id, out PrimitiveObject p) || !modes.TryGetValue(id, out var mode))
                continue;
            API.SplitSafe(mode, out var a, out var b);
            p.ColliderMode = a == PrimitiveObject.ColliderCreationMode.Unset ? b : a;
        }
    }

    private static void SetTriggerActions(InstanceList objects, InstanceDictionary<List<BaseTriggerActionData>> runtimeTriggerActions, ProgressUpdater updateProgress)
    {
        var count = objects.Count;
        var floatCount = (float) count;
        for (var i = 0; i < count; i++)
        {
            updateProgress?.Invoke($"Setting trigger actions ({i + 1} of {count})", i / floatCount);
            if (!objects.TryGet(i, out var id, out PrimitiveObject p))
                continue;
            if (runtimeTriggerActions.TryGetValue(id, out var actions))
                p.TriggerActions = actions.ToArray();
        }
    }

    #endregion

}
