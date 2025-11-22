using CefSharp;
using CefSharp.WinForms;
using LittleWarGameClient.UI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LittleWarGameClient.Helpers
{
    internal class ElementMessage
    {
        public string? Id { get; set; }
        public string? Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ButtonType? Type { get; set; }

        internal static void CallJSFunc(ChromiumWebBrowser receiver, string func, string args = "")
        {
            var script = $"addons.{func}({args})";
            receiver.ExecuteScriptAsync(script);
        }

        internal static void JSMessageReceived(object? sender, JavascriptMessageReceivedEventArgs e)
        {
            ElementMessage? msg = JsonSerializer.Deserialize<ElementMessage>((string)e.Message);
            if (msg != null)
            {
                switch (msg.Type)
                {
                    case ButtonType.FullScreen:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.ToggleFullscreen();
                        });
                        break;
                    case ButtonType.Exit:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.Close();
                        });
                        break;
                    case ButtonType.MouseLock:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.MouseLock(bool.Parse(msg.Value));
                            });
                        break;
                    case ButtonType.InitComplete:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.AddonsLoadedPostLogic();
                        });
                        break;
                    case ButtonType.VolumeChanging:
                        if (msg.Value != null)
                        {
                            GameForm.Instance.ChangeVolume(float.Parse(msg.Value));
                        }
                        break;
                    case ButtonType.VolumeChanged:
                        if (msg.Value != null)
                        {
                            GameForm.Instance.VolumeChangePostLogic(float.Parse(msg.Value));
                        }
                        break;
                    case ButtonType.OverlayType:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.OverlayChanged(int.Parse(msg.Value));
                            });
                        break;
                    case ButtonType.Reload:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.ReloadGame();
                        });
                        break;
                    case ButtonType.EnablePlugin:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.EnablePlugin(msg.Value);
                            });
                        break;
                    case ButtonType.DisablePlugin:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.DisablePlugin(msg.Value);
                            });
                        break;
                    case ButtonType.InstallPlugin:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.InstallPlugin(msg.Value);
                            });
                        break;
                    case ButtonType.UninstallPlugin:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.UninstallPlugin(msg.Value);
                            });
                        break;
                    case ButtonType.UpdatePlugin:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.UpdatePlugin(msg.Value);
                            });
                        break;
                    case ButtonType.BrowseAvailablePlugins:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.SendAvailablePlugins();
                        });
                        break;
                    case ButtonType.ViewInstalledPlugins:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.SendInstalledPluginsAndLatestVersions();
                        });
                        break;
                    case ButtonType.DisableAllPlugins:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.DisableAllPlugins(bool.Parse(msg.Value));
                            });
                        break;
                }
            }
        }
    }

    internal enum ButtonType
    {
        FullScreen,
        Exit,
        MouseLock,
        InitComplete,
        VolumeChanging,
        VolumeChanged,
        OverlayType,
        Reload,
        InstallPlugin,
        UninstallPlugin,
        EnablePlugin,
        DisablePlugin,
        UpdatePlugin,
        BrowseAvailablePlugins,
        ViewInstalledPlugins,
        DisableAllPlugins
    }
}
