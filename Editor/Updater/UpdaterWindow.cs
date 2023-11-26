using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Updater
{

    public sealed class UpdaterWindow : EditorWindow
    {

        private const string CheckForUpdatesFile = "slocDoNotCheckForUpdates";

        public static bool ShouldCheckForUpdates
        {
            get => !File.Exists(CheckForUpdatesFile);
            set
            {
                if (value)
                {
                    if (File.Exists(CheckForUpdatesFile))
                        File.Delete(CheckForUpdatesFile);
                }
                else if (!File.Exists(CheckForUpdatesFile))
                    File.Create(CheckForUpdatesFile).Dispose();
            }
        }

        [MenuItem("sloc/Updater")]
        public static void ShowWindow() => GetWindow<UpdaterWindow>(true, "sloc Updater");

        private static bool _checkForUpdates = ShouldCheckForUpdates;

        private void OnGUI()
        {
            var value = EditorGUILayout.Toggle(new GUIContent("Check for updates", "When Unity starts, a request will be made to GitHub if there are sloc updates available.\nIf a new version was released, you can choose to update or stay on the current version."), _checkForUpdates);
            if (value != _checkForUpdates)
            {
                _checkForUpdates = value;
                ShouldCheckForUpdates = value;
            }

            GUILayout.Label($"Current Version: {slocExporter.API.CurrentVersion}", EditorStyles.boldLabel);
            if (GUILayout.Button("Find Updates Now"))
                ProjectUpdater.UpdateBlocking();
        }

    }

}
