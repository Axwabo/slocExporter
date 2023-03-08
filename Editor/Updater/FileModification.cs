using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Editor.Updater.Responses;
using UnityEngine;

namespace Editor.Updater {

    [Serializable]
    public struct FileModification {

        public string path;

        public bool removed;

        public FileModification(string path, bool removed) {
            this.path = path;
            this.removed = removed;
        }

        private static bool IsTopLevelDirectory(ZipArchiveEntry arg) {
            var path = arg.FullName;
            var index = path.IndexOf('/');
            return index == -1 || index == path.Length - 1;
        }

        public static void PatchFiles(ProgressUpdater update, ChangedFile[] files, string assets) {
            using var zip = ZipFile.OpenRead(Constants.ArchiveFileName);
            var topDirectory = zip.Entries.FirstOrDefault(IsTopLevelDirectory);
            if (topDirectory == null) {
                Debug.LogWarning("[slocUpdater] Could not find top directory in archive");
                return;
            }

            var count = files.Length;
            var fc = (float) count;
            for (var i = 0; i < count; i++) {
                var file = files[i];
                var wasRemoved = file.status is "removed";
                var name = file.filename;
                var entry = wasRemoved ? null : zip.GetEntry(topDirectory + name);
                if (!wasRemoved && entry == null) {
                    Debug.LogWarning($"[slocUpdater] Could not find entry {name} in archive");
                    continue;
                }

                ProcessPatch(file, entry, wasRemoved, assets);
                update(name, (i + 1) / fc);
            }
        }

        private static void ProcessPatch(ChangedFile file, ZipArchiveEntry entry, bool wasRemoved, string assets) {
            var path = Path.Combine(assets, file.filename);
            if (wasRemoved) {
                if (File.Exists(path))
                    File.Delete(path);
                else
                    Debug.LogWarning($"[slocUpdater] Could not find file {file.filename} to remove");
                RemoveMetaFile(path);
            } else {
                var directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                    return;
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                entry.ExtractToFile(path, true);
            }
        }

        private static void RemoveMetaFile(string file) {
            var metaFile = file + ".meta";
            if (File.Exists(metaFile))
                File.Delete(metaFile);
        }

    }

}
