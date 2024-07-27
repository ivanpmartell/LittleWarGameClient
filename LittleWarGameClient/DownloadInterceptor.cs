using CefSharp;
using CefSharp.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleWarGameClient
{
    internal class DownloadInterceptor : DownloadHandler
    {
        protected override bool OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                var path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
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
                OverlayForm.OverlayMessage = $"Download progress: {downloadItem.PercentComplete}%";
            else if (downloadItem.IsComplete)
                OverlayForm.OverlayMessage = $"Download completed";
        }
    }
}
