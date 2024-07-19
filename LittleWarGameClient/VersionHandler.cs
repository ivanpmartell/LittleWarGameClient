using Microsoft.Web.WebView2.WinForms;
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

        public VersionHandler()
        {
            var productVersion = System.Windows.Forms.Application.ProductVersion.Split('+').First();
            CurrentVersion = new Version(productVersion);
            TryGetLatestVersionAsync();
            LatestVersionObtained += CheckForUpdate;
        }

        internal virtual void CheckForUpdate(object? sender, EventArgs e)
        {
            if (LatestVersion != null && RequiresUpdate())
                if (DialogResult.OK == MessageBox.Show("An update is available. Press OK to download it", "Update", MessageBoxButtons.OKCancel))
                    Process.Start(new ProcessStartInfo($"https://github.com/ivanpmartell/LittleWarGameClient/releases/tag/v{LatestVersion}") { UseShellExecute = true });
        }

        private bool RequiresUpdate()
        {
            int versionComparison = CurrentVersion.CompareTo(LatestVersion);
            if (versionComparison < 0)
                return true;
            return false;
        }

        private async void TryGetLatestVersionAsync()
        {
            try
            {
                LatestVersion = await TimeoutAfter(GetLatestGitHubVersion(), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception)
            {
                MessageBox.Show("Could not obtain the latest version");
            }
        }
        private async Task<Version> GetLatestGitHubVersion()
        {
            var client = new GitHubClient(new ProductHeaderValue("LWGClient"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("ivanpmartell", "LittleWarGameClient");
            return new Version(releases.First().TagName.Substring(1));
        }

        private async Task<TResult> TimeoutAfter<TResult>(Task<TResult> task, TimeSpan timeout)
        {
            // We need to be able to cancel the "timeout" task, so create a token source
            using (var cts = new CancellationTokenSource())
            {
                // Create the timeout task (don't await it)
                var timeoutTask = Task<TResult>.Delay(timeout, cts.Token);

                // Run the task and timeout in parallel, return the Task that completes first
                var completedTask = await Task<TResult>.WhenAny(task, timeoutTask).ConfigureAwait(false);

                if (completedTask == task)
                {
                    // Cancel the "timeout" task so we don't leak a Timer
                    cts.Cancel();
                    // await the task to bubble up any errors etc
                    return await task.ConfigureAwait(false);
                }
                else
                {
                    throw new TimeoutException($"Task timed out after {timeout}");
                }
            }
        }
    }
}
