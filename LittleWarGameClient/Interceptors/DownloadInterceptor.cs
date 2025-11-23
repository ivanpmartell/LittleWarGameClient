using CefSharp;
using CefSharp.Handler;
using LittleWarGameClient.Handlers;
using LittleWarGameClient.Helpers;
using LittleWarGameClient.UI;

namespace LittleWarGameClient.Interceptors
{
    internal class DownloadInterceptor : DownloadHandler
    {
        protected override bool OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    var downloadsDirPath = Path.Combine(ProcessHelper.Instance.ExeDirectory, "downloads", ProcessHelper.Instance.Profile);
                    var completePath = Path.Combine(downloadsDirPath, downloadItem.SuggestedFileName);
                    var n = 0;
                    while (File.Exists(completePath))
                    {
                        n++;
                        var ext = Path.GetExtension(downloadItem.SuggestedFileName);
                        var filenameNoExt = Path.GetFileNameWithoutExtension(downloadItem.SuggestedFileName);
                        completePath = Path.Combine(downloadsDirPath, $"{filenameNoExt}({n}){ext}");
                    }
                    callback.Continue(completePath, showDialog: false);
                    return true;
                }
            }
            return false;
        }

        protected override void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if (downloadItem.IsInProgress)
                OverlayHelper.Instance.AddOverlayMessage($"download{downloadItem.Id}", new Notification($"Download progress: {downloadItem.PercentComplete}%"));
            else if (downloadItem.IsComplete)
                OverlayHelper.Instance.AddOverlayMessage($"download{downloadItem.Id}", new Notification("Download completed"));
        }
    }
}
