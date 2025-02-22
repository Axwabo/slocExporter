using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using slocExporter.Objects;
using slocExporter.Readers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace slocExporter.Serialization.Exporting
{

    public sealed class FileExporter : IDisposable
    {

        private readonly ExportPreset _preset;

        private readonly BinaryWriter _writer;

        public FileExporter(string path, bool debug, ExportPreset preset, ProgressUpdater progress)
        {
            _preset = preset;
            _writer = new BinaryWriter(File.Open(path, FileMode.Create), Encoding.UTF8);
        }

        // TODO: refactor
        public void Export(bool selectedOnly)
        {
            var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(e => e.WithAllChildren()).ToList();
            var exportables = new Dictionary<GameObject, IExportable<slocGameObject>>();
            foreach (var o in gameObjects)
                if (o.TryIdentify(out var exportable))
                    exportables.Add(o, exportable);
            var slocObjects = new List<slocGameObject>();
            foreach (var (o, exportable) in exportables)
            {
                foreach (var component in o.GetComponents<Component>())
                    exportable.TryProcess(component);
                var exported = exportable.Export(o.GetInstanceID());
                if (exported == null)
                    continue;
                var t = o.transform;
                exported.Transform = t;
                var parent = t.parent;
                if (parent)
                    exported.ParentId = parent.gameObject.GetInstanceID();
                slocObjects.Add(exported);
            }

            _writer.Write(API.slocVersion);
            var count = slocObjects.Count;
            var header = new slocHeader(API.slocVersion, count, Attributes, _preset.defaultPrimitiveFlags);
            header.WriteTo(_writer);
            foreach (var o in slocObjects)
                o.WriteTo(_writer, header);
            EditorUtility.DisplayDialog("Export Completed", $"sloc created with {slocObjects.Count} object(s).", "OK");
        }

        public slocAttributes Attributes
        {
            get
            {
                var attributes = slocAttributes.None;
                if (_preset.lossyColors)
                    attributes |= slocAttributes.LossyColors;
                if (_preset.exportAllTriggerActions)
                    attributes |= slocAttributes.ExportAllTriggerActions;
                if (_preset.defaultPrimitiveFlags != PrimitiveObjectFlags.None)
                    attributes |= slocAttributes.DefaultFlags;
                return attributes;
            }
        }

        public void Dispose() => _writer?.Dispose();

    }

}
