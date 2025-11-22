using LittleWarGameClient.Helpers;
using LittleWarGameClient.UI;
using Octokit;
using System.Diagnostics;

namespace LittleWarGameClient.Handlers
{
    internal class VersionHandler
    {
        private readonly SettingsHandler settings;
        internal Version CurrentVersion { get; private set; }
        private Version? latestVersion;
        internal Version? LatestVersion
        {
            get => latestVersion;
            set
            {
                if (latestVersion != value)
                {
                    latestVersion = value;
                    LatestVersionObtained(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler LatestVersionObtained;

        internal VersionHandler(SettingsHandler s)
        {
            settings = s;
            var productVersion = System.Windows.Forms.Application.ProductVersion.Split('+').First();
            CurrentVersion = new Version(productVersion);
#if DEBUG
            CurrentVersion = new Version(0, 0, 0);
#endif
            LatestVersionObtained += CheckForUpdate;
            if (CanCheckForUpdate())
            {
                OverlayHelper.Instance.AddOverlayMessage("updateCheck", new UI.Notification("Checking for updates..."));
                new Thread(() =>
                {
                    PerformCheck();
                }).Start();
            }
        }

        private async void PerformCheck()
        {
            LatestVersion = await GetLatestGitHubVersion();
        }

        internal async virtual void CheckForUpdate(object? sender, EventArgs e)
        {
            if (LatestVersion != null)
            {
                if (RequiresUpdate())
                {
                    if (DialogResult.OK == MessageBox.Show("An update is available. Press OK to download it and exit the game", "Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly))
                    {
                        var updateUrl = $"https://github.com/ivanpmartell/LittleWarGameClient/releases/download/v{LatestVersion}/";
                        var env = "x86";
                        if (Environment.Is64BitProcess)
                            env = "x64";

                        if (LatestVersion.Minor != CurrentVersion.Minor)
                            updateUrl += $"lwg_client{env}.zip";
                        else
                            updateUrl += $"update_{env}.zip";
                        Process.Start(new ProcessStartInfo(updateUrl) { UseShellExecute = true });
                        GameForm.Instance.Close();
                    }
                    else
                        OverlayHelper.Instance.AddOverlayMessage("updateCancel", new UI.Notification("Update canceled"));
                }
                else
                    OverlayHelper.Instance.AddOverlayMessage("updateNA", new UI.Notification("No update required"));
            }
            else
                OverlayHelper.Instance.AddOverlayMessage("updateError", new UI.Notification("Network Error: Could not check for newer versions"));
            settings.SetLastUpdateChecked(DateTime.Now);
            await settings.SaveAsync();
        }

        private bool RequiresUpdate()
        {
            int versionComparison = CurrentVersion.CompareTo(LatestVersion);
            if (versionComparison < 0)
                return true;
            return false;
        }

        private bool CanCheckForUpdate()
        {
            var lastChecked = settings.GetLastUpdateChecked();
            var interval = settings.GetUpdateInterval();
            var dateToCheckForUpdates = lastChecked.AddHours(interval);
            if (DateTime.Now < dateToCheckForUpdates)
                return false;
            return true;
        }
        private async Task<Version?> GetLatestGitHubVersion(int retries = 3)
        {
            if (retries < 1)
                return null;
            try
            {
                var client = new GitHubClient(new ProductHeaderValue("LWGClient"));
                var release = await client.Repository.Release.GetLatest("ivanpmartell", "LittleWarGameClient");
                return new Version(release.TagName.Substring(1));
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                return await GetLatestGitHubVersion(retries - 1);
            }
        }
    }
}
