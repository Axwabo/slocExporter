using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Editor.Updater.Responses;
using UnityEngine;

namespace Editor.Updater
{

    [Serializable]
    public struct FileModification
    {

        public string path;

        public bool removed;

        public FileModification(string path, bool removed)
        {
            this.path = path;
            this.removed = removed;
        }

        private static bool IsTopLevelDirectory(ZipArchiveEntry arg)
        {
            var path = arg.FullName;
            var index = path.IndexOf('/');
            return index == -1 || index == path.Length - 1;
        }

        public static void PatchFiles(ProgressUpdater update, ChangedFile[] files, string assets)
        {
            using var zip = ZipFile.OpenRead(Constants.ArchiveFileName);
            var topDirectory = zip.Entries.FirstOrDefault(IsTopLevelDirectory);
            if (topDirectory == null)
            {
                Debug.LogWarning("[slocUpdater] Could not find top directory in archive");
                return;
            }

            var projectFile = File.Exists(Constants.ProjectFileName) ? File.ReadAllLines(Constants.ProjectFileName).ToList() : new List<string>();
            var editorProjectFile = File.Exists(Constants.EditorProjectFileName) ? File.ReadAllLines(Constants.EditorProjectFileName).ToList() : new List<string>();
            var count = files.Length;
            var fc = (float) count;
            for (var i = 0; i < count; i++)
            {
                var file = files[i];
                var wasRemoved = file.status is "removed";
                var name = file.filename;
                var entry = wasRemoved ? null : zip.GetEntry(topDirectory + name);
                if (!wasRemoved && entry == null)
                {
                    Debug.LogWarning($"[slocUpdater] Could not find entry {name} in archive");
                    continue;
                }

                ProcessPatch(file, entry, wasRemoved, assets, projectFile, editorProjectFile);
                update(name, (i + 1) / fc);
            }

            if (projectFile.Count != 0)
                File.WriteAllLines(Constants.ProjectFileName, projectFile);
            if (editorProjectFile.Count != 0)
                File.WriteAllLines(Constants.EditorProjectFileName, editorProjectFile);
        }

        private static void ProcessPatch(ChangedFile file, ZipArchiveEntry entry, bool wasRemoved, string assets, List<string> projectFile, List<string> editorProjectFile)
        {
            var path = Path.GetFullPath(Path.Combine(assets, file.filename));
            if (wasRemoved)
            {
                if (File.Exists(path))
                    File.Delete(path);
                else
                    Debug.LogWarning($"[slocUpdater] Could not find file {file.filename} to remove");
                RemoveMetaFile(path);
            }
            else
            {
                var directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                    return;
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                entry.ExtractToFile(path, true);
            }

            ModifyCsproj(path, wasRemoved, assets, path.Replace('\\', '/').Contains("Editor/") ? editorProjectFile : projectFile);
        }

        private static readonly Regex CsprojRegex = new(@"<Compile\s?Include=""(.*)""\s?/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static void ModifyCsproj(string path, bool wasRemoved, string assets, List<string> list)
        {
            if (!path.EndsWith(".cs") || list.Count == 0)
                return;
            var pathFromRoot = Path.GetRelativePath(Directory.GetParent(assets)!.FullName, path).Replace('\\', '/');
            var existingIndex = list.FindIndex(s =>
            {
                var match = CsprojRegex.Match(s);
                return match.Success && match.Groups[1].Value.Replace('\\', '/') == pathFromRoot;
            });
            if (wasRemoved)
            {
                if (existingIndex != -1)
                    list.RemoveAt(existingIndex);
                return;
            }

            if (existingIndex != -1)
                return;
            var index = list.FindIndex(s => s.Contains("<Compile"));
            if (index != -1)
                list.Insert(index, $"<Compile Include=\"{pathFromRoot.Replace('/', Path.DirectorySeparatorChar)}\" />");
        }

        private static void RemoveMetaFile(string file)
        {
            var metaFile = file + ".meta";
            if (File.Exists(metaFile))
                File.Delete(metaFile);
        }

    }

}
