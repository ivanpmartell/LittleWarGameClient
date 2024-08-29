using CefSharp;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LittleWarGameClient.Handlers
{
    internal class VersionHandler
    {
        readonly SettingsHandler settings;
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

        public VersionHandler(SettingsHandler s)
        {
            settings = s;
            var productVersion = System.Windows.Forms.Application.ProductVersion.Split('+').First();
            CurrentVersion = new Version(productVersion);
#if DEBUG
            CurrentVersion = new Version(0, 0, 0);
#endif
            LatestVersionObtained += CheckForUpdate;
            new Thread(() =>
            {
                PerformCheck();
            }).Start();
        }

        private async void PerformCheck()
        {
            if (CanCheckForUpdate())
            {
                OverlayForm.Instance.AddOverlayMessage("updateCheck", new Notification("Checking for updates..."));
                LatestVersion = await GetLatestGitHubVersion();
            }
        }

        internal async virtual void CheckForUpdate(object? sender, EventArgs e)
        {
            if (LatestVersion != null && RequiresUpdate())
                if (DialogResult.OK == MessageBox.Show("An update is available. Press OK to download it and exit the game", "Update", MessageBoxButtons.OKCancel))
                {
                    var updateUrl = $"https://github.com/ivanpmartell/LittleWarGameClient/releases/download/v{LatestVersion}/";
                    if (Environment.Is64BitProcess)
                        updateUrl += "update_x64.zip";
                    else
                        updateUrl += "update_x86.zip";
                    Process.Start(new ProcessStartInfo(updateUrl) { UseShellExecute = true });
                    GameForm.Instance.Close();
                }
            settings.SetLastUpdateChecked(DateTime.Now.Date);
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
            var dateToCheckForUpdates = lastChecked.AddDays(interval);
            if (DateTime.Now < dateToCheckForUpdates)
                return false;
            return true;
        }
        private async Task<Version?> GetLatestGitHubVersion(int retries = 3)
        {
            if (retries < 1)
            {
                OverlayForm.Instance.AddOverlayMessage("updateError", new Notification("Network Error: Could not check for newer versions"));
                return null;
            }
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
