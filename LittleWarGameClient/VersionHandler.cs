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

namespace LittleWarGameClient
{
    internal class VersionHandler
    {
        readonly Settings settings;
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

        public VersionHandler(Settings s)
        {
            settings = s;
            var productVersion = System.Windows.Forms.Application.ProductVersion.Split('+').First();
            CurrentVersion = new Version(productVersion);
            LatestVersionObtained += CheckForUpdate;
            PerformCheck();
        }

        private async void PerformCheck()
        {
            if (CanCheckForUpdate())
            {
                LatestVersion = await TryGetLatestVersionAsync();
            }
        }

        internal virtual void CheckForUpdate(object? sender, EventArgs e)
        {
            if (LatestVersion != null && RequiresUpdate())
                if (DialogResult.OK == MessageBox.Show("An update is available. Press OK to download it and exit the game", "Update", MessageBoxButtons.OKCancel))
                {
                    Process.Start(new ProcessStartInfo($"https://github.com/ivanpmartell/LittleWarGameClient/releases/download/v{LatestVersion}/lwg_client.zip") { UseShellExecute = true });
                    System.Windows.Forms.Application.Exit();
                }
            settings.SetLastUpdateChecked(DateTime.Now.Date);
            settings.SaveAsync();
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

        private async Task<Version?> TryGetLatestVersionAsync()
        {
            try
            {
                return await TimeoutAfter(GetLatestGitHubVersion(), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception)
            {
                OverlayForm.OverlayMessage = "Could not obtain the latest version";
                return null;
            }
        }
        private async Task<Version> GetLatestGitHubVersion()
        {
            var client = new GitHubClient(new ProductHeaderValue("LWGClient"));
            var release = await client.Repository.Release.GetLatest("ivanpmartell", "LittleWarGameClient");
            return new Version(release.TagName.Substring(1));
        }

        private async Task<TResult> TimeoutAfter<TResult>(Task<TResult> task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            var timeoutTask = Task<TResult>.Delay(timeout, cts.Token);
            var completedTask = await Task<TResult>.WhenAny(task, timeoutTask).ConfigureAwait(false);
            if (completedTask == task)
            {
                cts.Cancel();
                return await task.ConfigureAwait(false);
            }
            else
            {
                throw new TimeoutException($"Task timed out after {timeout}");
            }
        }
    }
}
