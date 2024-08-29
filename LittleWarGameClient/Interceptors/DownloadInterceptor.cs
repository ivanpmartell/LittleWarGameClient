using CefSharp;
using CefSharp.Handler;

namespace LittleWarGameClient.Interceptors
{
    internal class DownloadInterceptor : DownloadHandler
    {
        protected override bool OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                var path = Path.GetDirectoryName(Application.ExecutablePath);
                using (callback)
                {
                    var downloadsDirPath = Path.Join(path, "downloads");
                    var completePath = Path.Join(downloadsDirPath, downloadItem.SuggestedFileName);
                    var n = 0;
                    while (File.Exists(completePath))
                    {
                        n++;
                        var ext = Path.GetExtension(downloadItem.SuggestedFileName);
                        var filenameNoExt = Path.GetFileNameWithoutExtension(downloadItem.SuggestedFileName);
                        completePath = Path.Join(downloadsDirPath, $"{filenameNoExt}({n}){ext}");
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
                OverlayForm.Instance.AddOverlayMessage($"download{downloadItem.Id}", new Notification($"Download progress: {downloadItem.PercentComplete}%"));
            else if (downloadItem.IsComplete)
                OverlayForm.Instance.AddOverlayMessage($"download{downloadItem.Id}", new Notification("Download completed"));
        }
    }
}
