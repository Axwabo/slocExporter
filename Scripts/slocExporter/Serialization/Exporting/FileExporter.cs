using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.Readers;
using UnityEngine;

namespace slocExporter.Serialization.Exporting
{

    public readonly struct FileExporter : IDisposable
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

        public int Export(bool selectedOnly)
        {
            Log("Collecting objects");
            _progress("Collecting objects to export", -1);
            var gameObjects = ExportCollector.GetObjects(selectedOnly, _preset, _debug);

            Log("Identifying objects");
            var exportables = new Dictionary<GameObject, IExportable<slocGameObject>>();
            var i = 0;
            var gameObjectsCount = gameObjects.Count;
            foreach (var o in gameObjects)
            {
                _progress.Count(++i, gameObjectsCount, "Identifying objects {2:P2} ({0} of {1})");
                var exportable = o.ToExportable();
                if (_debug)
                    Debug.Log($"Created {exportable}", o);
                exportables.Add(o, exportable);
            }

            Log("Processing & exporting");
            var context = ExportContext.From(_preset, _debug);
            var slocObjects = exportables.ProcessAndExportObjects(context, _progress);

            Log("Writing file");
            _writer.Write(API.slocVersion);
            var slocObjectsCount = slocObjects.Count;
            var header = new slocHeader(API.slocVersion, slocObjectsCount, context.Attributes, context.DefaultPrimitiveFlags);
            header.WriteTo(_writer);

            for (i = 0; i < slocObjects.Count; i++)
            {
                _progress.Count(i, slocObjectsCount, "Writing objects {2:P2} ({0} of {1})");
                slocObjects[i].WriteTo(_writer, header);
            }

            return slocObjectsCount;
        }

        private void Log(string message)
        {
            if (_debug)
                Debug.Log(message);
        }

        public void Dispose() => _writer?.Dispose();

    }

}
