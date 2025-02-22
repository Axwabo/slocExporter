using System;
using System.IO;
using System.Text;

namespace slocExporter.Serialization.Exporting
{

    public sealed class FileExporter : IDisposable
    {

        private readonly BinaryWriter _writer;

        public FileExporter(string path, bool debug, ExportPreset preset, ProgressUpdater progress)
            => _writer = new BinaryWriter(File.Open(path, FileMode.Create), Encoding.UTF8);

        public void Export(bool selectedOnly)
        {
            // TODO
        }

        public void Dispose() => _writer?.Dispose();

    }

}
