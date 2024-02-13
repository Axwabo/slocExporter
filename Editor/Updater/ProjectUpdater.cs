using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Editor.Updater.Responses;
using slocExporter;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static Editor.Updater.Constants;

namespace Editor.Updater
{

    [InitializeOnLoad]
    public static class ProjectUpdater
    {

        private const string InitDone = "slocUnityInitDone";

        private static UnityWebRequest _request;
        private static bool _canceled;
        private static TagResponse[] _tags;
        private static ChangedFile[] _files;
        private static string _assets;
        private static int _asyncProgressId;
        private static bool _isDone;

        #region Common Methods

        private static bool Sending => _request != null;

        private static bool IsUpToDate(string latestVersion) =>
            Version.TryParse(latestVersion, out var latest)
            && Version.TryParse(API.CurrentVersion, out var current)
            && latest <= current;

        private static bool ResetUpdate()
        {
            if (Sending)
                return false;
            if (Progress.Exists(_asyncProgressId))
                Progress.Remove(_asyncProgressId);
            _canceled = false;
            _tags = null;
            _files = null;
            _assets = null;
            return true;
        }

        private static bool TryParseJson<T>(out T result)
        {
            var text = _request?.downloadHandler?.text;
            if (!string.IsNullOrEmpty(text))
                try
                {
                    result = JsonUtility.FromJson<T>(text);
                    return true;
                }
                catch (Exception e)
                {
                    Warn("Failed to parse JSON response:\n" + e);
                }

            result = default;
            return false;
        }

        #endregion

        // Startup initializer
        static ProjectUpdater()
        {
            if (SessionState.GetBool(InitDone, false))
                return;
            SessionState.SetBool(InitDone, true);
            if (UpdaterWindow.ShouldCheckForUpdates)
                UpdateAsync();
        }

        public static async void UpdateAsync()
        {
            if (!ResetUpdate())
                return;
            try
            {
                _asyncProgressId = ProgressStart("Preparing sloc update", CheckingForUpdates);
                var checkedForUpdates = await SendRequestAsync(string.Format(Tags, GitHubUsername, Repository), CheckingForUpdates);
                if (!checkedForUpdates || !CompleteUpdateCheck(true) || !TryAskForUpdate(out var version, true))
                    return;

                ProgressBg(1, 4, ReceivingChanges);
                var receivedChanges = await SendRequestAsync(string.Format(Compare, GitHubUsername, Repository, API.CurrentVersion, version), ReceivingChanges);
                if (!receivedChanges || !ChangesReceived(true))
                    return;

                ProgressBg(3, 4, DownloadingFiles);
                var downloadedFiles = await DownloadFilesAsync();
                if (!downloadedFiles || !ValidateDownloadedFile())
                    return;

                Progress.Remove(_asyncProgressId);
                _asyncProgressId = ProgressStart("Updating sloc", "Patching...");
                _assets = Application.dataPath;
                await Task.Run(() => FileModification.PatchFiles(UpdateProgressBackground, _files!, _assets));
                API.CurrentVersion = version;
                Log($"Successfully updated to version {version}!");
                Progress.Report(_asyncProgressId, 1f, "Update complete!");
                Progress.Finish(_asyncProgressId);
                EditorUtility.DisplayDialog("sloc Update", "Update complete!", "Nice");
            }
            catch (Exception e)
            {
                Debug.LogError("[slocUpdater] " + e);
                Finish(Progress.Status.Failed);
                EditorUtility.DisplayDialog("sloc Update", "Update failed! See the Debug log for details.", "Bruh");
            }
            finally
            {
                DeleteFile();
            }
        }

        public static void UpdateBlocking()
        {
            if (!ResetUpdate())
                return;
            try
            {
                var checkedForUpdates = SendRequestBlocking(string.Format(Tags, GitHubUsername, Repository), CheckingForUpdates);
                if (!checkedForUpdates || !CompleteUpdateCheck(false) || !TryAskForUpdate(out var version, false))
                    return;

                var receivedChanges = SendRequestBlocking(string.Format(Compare, GitHubUsername, Repository, API.CurrentVersion, version), ReceivingChanges);
                if (!receivedChanges || !ChangesReceived(false))
                    return;

                var downloadedFiles = DownloadFilesBlocking();
                if (!downloadedFiles || !ValidateDownloadedFile())
                    return;

                ProgressNoCancel("Patching...", -1f);
                _assets = Application.dataPath;
                FileModification.PatchFiles(UpdateProgressBlocking, _files!, _assets);
                API.CurrentVersion = version;
                Log($"Successfully updated to version {version}!");
                EditorUtility.DisplayDialog("sloc Update", "Update complete!", "Nice");
            }
            catch (Exception e)
            {
                Debug.LogError("[slocUpdater] " + e);
                EditorUtility.DisplayDialog("sloc Update", "Update failed! See the Debug log for details.", "Bruh");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                DeleteFile();
            }
        }

        #region Request Handling

        private static void CreateRequest(string url, Action<UnityWebRequest> setup = null)
        {
            _isDone = false;
            _canceled = false;
            _request = UnityWebRequest.Get(url);
            _request.SetRequestHeader("User-Agent", "Axwabo-slocExporter-Updater");
            setup?.Invoke(_request);
        }

        private static Task<bool> SendRequestAsync(string url, string description, Action<UnityWebRequest> setup = null)
        {
            CreateRequest(url, setup);
            _request.SendWebRequest().completed += _ => _isDone = true;
            return Task.Run(() => WaitForRequestCompletion(description, true));
        }

        private static bool SendRequestBlocking(string url, string description, Action<UnityWebRequest> setup = null)
        {
            CreateRequest(url, setup);
            _request.SendWebRequest();
            return WaitForRequestCompletion(description, false);
        }

        private static bool WaitForRequestCompletion(string title, bool silent)
        {
            while (Sending)
            {
                if (silent ? _isDone : _request.result != UnityWebRequest.Result.InProgress)
                    break;
                Thread.Sleep(50);
                if (!ProgressCancel(title, -1f, !silent))
                    continue;
                _canceled = true;
                _request.Abort();
                Dispose(false);
                Finish(Progress.Status.Canceled);
                return false;
            }

            return true;
        }

        private static void Dispose(bool clearProgressBar)
        {
            if (clearProgressBar)
                EditorUtility.ClearProgressBar();
            _request?.Dispose();
            _request = null;
        }

        #endregion

        #region Download

        private static Task<bool> DownloadFilesAsync() => SendRequestAsync(_tags[0].zipball_url, DownloadingFiles, SetDownloadHandler);

        private static bool DownloadFilesBlocking() => SendRequestBlocking(_tags[0].zipball_url, DownloadingFiles, SetDownloadHandler);

        private static void SetDownloadHandler(UnityWebRequest request) =>
            request.downloadHandler = new DownloadHandlerFile(Path.Combine(Directory.GetCurrentDirectory(), ArchiveFileName))
            {
                removeFileOnAbort = true
            };

        private static bool ValidateDownloadedFile()
        {
            if (File.Exists(ArchiveFileName))
                return true;
            Finish(Progress.Status.Failed);
            Debug.LogError("[slocUpdater] Failed to download update files");
            EditorUtility.DisplayDialog(ReceiveFailed, "Failed to download update files", "OK");
            return false;
        }

        private static void DeleteFile()
        {
            if (File.Exists(ArchiveFileName))
                File.Delete(ArchiveFileName);
        }

        #endregion

        #region Checks

        private static bool CompleteUpdateCheck(bool silent)
        {
            if (_canceled)
                return false;
            var requestResult = _request.result;
            switch (requestResult)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Finish(Progress.Status.Failed);
                    if (HandleStandardError(CheckFailed))
                        break;
                    var message = $"Couldn't check for updates\n{requestResult}: {_request.error}";
                    EditorUtility.DisplayDialog(CheckFailed, message, "OK");
                    Warn(message);
                    break;
                case UnityWebRequest.Result.Success:
                    if (silent)
                        ProgressBg(1, 4, null);
                    _tags = ResponseArray<TagResponse>.Parse(_request.downloadHandler.text);
                    break;
            }

            Dispose(!silent);
            return _tags is {Length: not 0};
        }

        private static bool ChangesReceived(bool silent)
        {
            if (_canceled)
                return false;
            var requestResult = _request.result;
            switch (requestResult)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Finish(Progress.Status.Failed);
                    if (HandleStandardError(ReceiveFailed))
                        break;
                    var message = $"Couldn't receive file changes\n{requestResult}: {_request.error}";
                    Warn(message);
                    EditorUtility.DisplayDialog(ReceiveFailed, message, "OK");
                    break;
                case UnityWebRequest.Result.Success:
                    ChangesRequestSuccess();
                    break;
            }

            Dispose(!silent);
            return _files is {Length: not 0};
        }

        private static void ChangesRequestSuccess()
        {
            Progress.Report(_asyncProgressId, 2, 4);
            if (!TryParseJson(out ChangesResponse r))
            {
                Finish(Progress.Status.Failed);
                EditorUtility.DisplayDialog(ReceiveFailed, "Couldn't parse file changes\nServer sent an invalid response", "OK");
                return;
            }

            if (r.message != null)
            {
                Finish(Progress.Status.Failed);
                Warn("Could not receive file changes: " + r.message);
                EditorUtility.DisplayDialog(ReceiveFailed, r.message, "OK");
                return;
            }

            _files = r.files;
        }

        private static bool TryAskForUpdate(out string latest, bool silent)
        {
            if (_tags is not {Length: not 0})
            {
                Finish(Progress.Status.Failed);
                Warn("No update information received");
                EditorUtility.DisplayDialog(CheckFailed, "No update information received", "OK");
                latest = null;
                return false;
            }

            var version = latest = _tags[0].name.TrimStart('v', 'V');
            if (IsUpToDate(version))
            {
                ProgressBg(1, 4, "sloc is up to date");
                Finish(Progress.Status.Succeeded);
                if (!silent)
                    EditorUtility.DisplayDialog("sloc update", "sloc is up to date.", "OK");
                Log("sloc is up to date.");
                return false;
            }

            if (silent)
                ProgressBg(1, 4, "Waiting for user confirmation...");
            else
                EditorUtility.DisplayProgressBar("sloc Update", "Waiting for user confirmation...", -1);
            var update = EditorUtility.DisplayDialog("sloc update", $"An update is available. Would you like to install it?\nYour version:{API.CurrentVersion}\nLatest version: {version}", "Yes", "Skip");
            if (!update)
                Finish(Progress.Status.Canceled);
            return update;
        }

        #endregion

        #region Error Handling

        private static bool HandleStandardError(string title)
        {
            var reset = _request?.GetResponseHeader("x-ratelimit-reset");
            var remaining = _request?.GetResponseHeader("x-ratelimit-remaining");
            if (!string.IsNullOrEmpty(remaining) && int.TryParse(remaining, out var count) && count > 0)
                return HandleErrorMessage(title);
            if (reset == null || !int.TryParse(reset, out var ms))
                return HandleErrorMessage(title);
            var time = DateTime.UnixEpoch.ToLocalTime().AddSeconds(ms);
            if (time < DateTime.Now)
                return HandleErrorMessage(title);
            var t = time.ToString(CultureInfo.CurrentCulture);
            Warn($"GitHub API rate limit exceeded. Please try again after {t}.");
            EditorUtility.DisplayDialog(title, string.Format(RateLimit, t), "OK");
            return true;
        }

        private static bool HandleErrorMessage(string title)
        {
            if (!TryParseJson(out ErrorMessage msg))
                return false;
            Warn("Update failed\nGitHub API response:" + msg.message);
            EditorUtility.DisplayDialog(title, msg.message, "OK");
            return true;
        }

        #endregion

        #region Progress

        private static void ProgressNoCancel(string description, float progress, bool show = true)
        {
            if (show)
                EditorUtility.DisplayProgressBar("sloc Update", description, progress);
        }

        private static bool ProgressCancel(string description, float progress, bool show = true) => show && EditorUtility.DisplayCancelableProgressBar("sloc Update", description, progress);

        private static int ProgressStart(string title, string description)
        {
            var id = Progress.Start(title, description, Progress.Options.Sticky);
            Progress.Report(id, -1f);
            return id;
        }

        private static void Finish(Progress.Status status)
        {
            if (Progress.Exists(_asyncProgressId))
                Progress.Finish(_asyncProgressId, status);
        }

        private static void ProgressBg(int current, int total, string description) => Progress.Report(_asyncProgressId, current, total, description);

        private static void UpdateProgressBackground(string message, float progress) => Progress.Report(_asyncProgressId, progress, message);

        private static void UpdateProgressBlocking(string message, float progress) => ProgressNoCancel(message, progress);

        #endregion

        #region Log

        private static void Log(object message) => Debug.Log("[slocUpdater] " + message);

        private static void Warn(string message) => Debug.LogWarning("[slocUpdater] " + message);

        #endregion

    }

}
