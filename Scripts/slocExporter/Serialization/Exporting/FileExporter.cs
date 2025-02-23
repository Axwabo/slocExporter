using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using slocExporter.Objects;
using slocExporter.Readers;
using UnityEditor;
using UnityEngine;

namespace slocExporter.Serialization.Exporting
{

    public sealed class FileExporter : IDisposable
    {

        private readonly bool _debug;
        private readonly ExportPreset _preset;
        private readonly ProgressUpdater _progress;

        private readonly BinaryWriter _writer;

        public FileExporter(string path, bool debug, ExportPreset preset, ProgressUpdater progress)
        {
            _debug = debug;
            _preset = preset;
            _progress = progress;
            _writer = new BinaryWriter(File.Open(path, FileMode.Create), Encoding.UTF8);
        }

        public void Export(bool selectedOnly)
        {
            _progress("Collecting objects to export", -1);
            var gameObjects = ExportCollector.GetObjects(selectedOnly);

            var exportables = new Dictionary<GameObject, IExportable<slocGameObject>>();
            foreach (var o in gameObjects)
                if (o.TryIdentify(out var exportable))
                    exportables.Add(o, exportable);

            var context = ExportContext.From(_preset);
            var slocObjects = exportables.ProcessAndExportObjects(context);

            _writer.Write(API.slocVersion);
            var count = slocObjects.Count;
            var header = new slocHeader(API.slocVersion, count, context.Attributes, context.DefaultPrimitiveFlags);
            header.WriteTo(_writer);

            foreach (var o in slocObjects)
                o.WriteTo(_writer, header);

            EditorUtility.DisplayDialog("Export Completed", $"sloc created with {slocObjects.Count} object(s).", "OK");
        }

        public void Dispose() => _writer?.Dispose();

    }

}
