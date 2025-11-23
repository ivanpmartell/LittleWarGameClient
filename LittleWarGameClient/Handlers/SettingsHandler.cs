using IniFile;
using LittleWarGameClient.Helpers;

namespace LittleWarGameClient.Handlers
{
    internal class SettingsHandler
    {
        private readonly string fileName;
        private const OverlayType defaultOverlayType = OverlayType.OpenGL;
        private const int defaultWidth = 1280;
        private const int defaultHeight = 720;
        private const int defaultLoadingImageWidth = 200;
        private const int defaultLoadingImageHeight = 200;
        private const int defaultLoadingImageLocationLeft = 532;
        private const int defaultLoadingImageLocationTop = 70;
        private const string defaultLoadingImagePath = "";
        private const string defaultPluginRepoReleaseVersion = "0.0.0";
        private const bool defaultDebugMode = false;
        private const bool defaultDisableAllPlugins = false;
        private const bool defaultFullscreen = false;
        private const bool defaultMouseLock = false;
        private const int defaultUpdateInterval = 1;
        private const double defaultVolume = 1.0;
        private readonly DateTime defaultUpdateLastChecked = DateTime.MinValue;
        private const Keys defaultOptionsMenuHotkey = Keys.F10;
        private const Keys defaultFriendsMenuHotkey = Keys.F9;
        private const Keys defaultChatHistoryMenuHotkey = Keys.F11;
        private const Keys defaultFullscreenHotkey = Keys.F8;
        private readonly Ini settings;
        private readonly SettingsHelper helper;

        internal SettingsHandler()
        {
            var settingsDirectory = "settings";
            if (!Directory.Exists(settingsDirectory))
                Directory.CreateDirectory(settingsDirectory);
            fileName = Path.Join(settingsDirectory, $"{ProcessHelper.Instance.Profile}.ini");
            if (!File.Exists(fileName))
                settings = CreateDefaultIniFile();
            else
                settings = new Ini(fileName);
            helper = new SettingsHelper(settings);
            Init();
        }

        private async void Init()
        {
            SetOverlayType(GetOverlayType());
            SetMouseLock(GetMouseLock());
            SetFullScreen(GetFullScreen());
            SetWindowSize(GetWindowSize());
            SetLastUpdateChecked(GetLastUpdateChecked());
            SetUpdateInterval(GetUpdateInterval());
            SetOptionsMenuHotkey(GetOptionsMenuHotkey());
            SetFriendsMenuHotkey(GetFriendsMenuHotkey());
            SetChatHistoryMenuHotkey(GetChatHistoryMenuHotkey());
            SetFullscreenHotkey(GetFullscreenHotkey());
            SetVolume(GetVolume());
            SetLoadingImagePath(GetLoadingImagePath());
            SetLoadingImageLocation(GetLoadingImageLocation());
            SetLoadingImageSize(GetLoadingImageSize());
            SetDisableAllPlugins(GetDisableAllPlugins());
            SetDebugMode(GetDebugMode());
            SetPluginRepoReleaseVersion(GetPluginRepoReleaseVersion());
            await SaveAsync();
        }

        private Ini CreateDefaultIniFile()
        {
            var settings = new Ini
            {
                new Section("Window")
                {
                    new Property("width", defaultWidth),
                    new Property("height", defaultHeight),
                    new Property("fullscreen", defaultFullscreen)
                }
            };
            return settings;
        }

        internal void Save()
        {
            settings.SaveTo(fileName);
        }

        internal async Task SaveAsync()
        {
            try
            {
                using FileStream stream = new(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                await settings.SaveToAsync(stream);
            }
            catch
            {
                Thread.Sleep(50);
                await SaveAsync();
            }
        }

        internal void SetOverlayType(OverlayType value)
        {
            helper.SetVariable("Window", "overlay", value);
        }

        internal OverlayType GetOverlayType()
        {
            return helper.GetVariable("Window", "overlay", defaultOverlayType);
        }

        internal void SetMouseLock(bool value)
        {
            helper.SetVariable("Mouse", "lock", value);
        }

        public bool GetMouseLock()
        {
            return helper.GetVariable("Mouse", "lock", defaultMouseLock);
        }

        internal void SetFullScreen(bool value)
        {
            helper.SetVariable("Window", "fullscreen", value);
        }

        public bool GetFullScreen()
        {
            return helper.GetVariable("Window", "fullscreen", defaultFullscreen);
        }

        internal void SetWindowSize(Size size)
        {
            helper.SetVariable("Window", "width", size.Width);
            helper.SetVariable("Window", "height", size.Height);
        }

        public Size GetWindowSize()
        {
            var width = helper.GetVariable("Window", "width", defaultWidth);
            var height = helper.GetVariable("Window", "height", defaultHeight);
            return new Size(width, height);
        }

        internal void SetLastUpdateChecked(DateTime value)
        {
            helper.SetVariable("Update", "lastChecked", value);
        }

        public DateTime GetLastUpdateChecked()
        {
            return helper.GetVariable("Update", "lastChecked", defaultUpdateLastChecked);
        }

        internal void SetUpdateInterval(int value)
        {
            helper.SetVariable("Update", "interval", value);
        }

        public int GetUpdateInterval()
        {
            return helper.GetVariable("Update", "interval", defaultUpdateInterval);
        }

        internal void SetOptionsMenuHotkey(Keys value)
        {
            helper.SetVariable("Hotkeys", "optionsMenu", value);
        }

        [Hotkey(FuncToCall = "OptionsMenuHotkeyFunc", JSFuncToCall = "addOptionsMenuHotkey")]
        public Keys GetOptionsMenuHotkey()
        {
            return helper.GetVariable("Hotkeys", "optionsMenu", defaultOptionsMenuHotkey);
        }

        internal void SetFriendsMenuHotkey(Keys value)
        {
            helper.SetVariable("Hotkeys", "friendsMenu", value);
        }

        [Hotkey(FuncToCall = "FriendsHotkeyFunc", JSFuncToCall = "addFriendsMenuHotkey")]
        public Keys GetFriendsMenuHotkey()
        {
            return helper.GetVariable("Hotkeys", "friendsMenu", defaultFriendsMenuHotkey);
        }

        internal void SetChatHistoryMenuHotkey(Keys value)
        {
            helper.SetVariable("Hotkeys", "chatHistoryMenu", value);
        }

        [Hotkey(FuncToCall = "ChatHistoryHotkeyFunc", JSFuncToCall = "addChatHistoryHotkey")]
        public Keys GetChatHistoryMenuHotkey()
        {
            return helper.GetVariable("Hotkeys", "chatHistoryMenu", defaultChatHistoryMenuHotkey);
        }

        internal void SetFullscreenHotkey(Keys value)
        {
            helper.SetVariable("Hotkeys", "fullscreen", value);
        }

        [Hotkey(FuncToCall = "FullscreenHotkeyFunc", JSFuncToCall = "addFullscreenBtnHotkey")]
        public Keys GetFullscreenHotkey()
        {
            return helper.GetVariable("Hotkeys", "fullscreen", defaultFullscreenHotkey);
        }

        internal void SetVolume(double value)
        {
            helper.SetVariable("Audio", "volume", value);
        }

        public double GetVolume()
        {
            return helper.GetVariable("Audio", "volume", defaultVolume);
        }

        internal void SetDisableAllPlugins(bool value)
        {
            helper.SetVariable("Plugins", "disable", value);
        }

        public bool GetDisableAllPlugins()
        {
            return helper.GetVariable("Plugins", "disable", defaultDisableAllPlugins);
        }

        internal void SetDebugMode(bool value)
        {
            helper.SetVariable("Plugins", "debug", value);
        }

        public bool GetDebugMode()
        {
            return helper.GetVariable("Plugins", "debug", defaultDebugMode);
        }

        internal void SetPluginRepoReleaseVersion(Version value)
        {
            helper.SetVariable("Plugins", "release", value);
        }

        public Version GetPluginRepoReleaseVersion()
        {
            return helper.GetVariable("Plugins", "release", new Version(defaultPluginRepoReleaseVersion));
        }

        internal void SetLoadingImageLocation(Point value)
        {
            helper.SetVariable("Loader", "image_x", value.X);
            helper.SetVariable("Loader", "image_y", value.Y);
        }

        public Point GetLoadingImageLocation()
        {
            int left = helper.GetVariable("Loader", "image_x", defaultLoadingImageLocationLeft);
            int top = helper.GetVariable("Loader", "image_y", defaultLoadingImageLocationTop);
            return new Point(left, top);
        }

        internal void SetLoadingImageSize(Size value)
        {
            helper.SetVariable("Loader", "image_width", value.Width);
            helper.SetVariable("Loader", "image_height", value.Height);
        }

        public Size GetLoadingImageSize()
        {
            int width = helper.GetVariable("Loader", "image_width", defaultLoadingImageWidth);
            int height = helper.GetVariable("Loader", "image_height", defaultLoadingImageHeight);
            return new Size(width, height);
        }

        internal void SetLoadingImagePath(string value)
        {
            helper.SetVariable("Loader", "image_path", value);
        }

        public string GetLoadingImagePath()
        {
            return helper.GetVariable("Loader", "image_path", defaultLoadingImagePath);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal class Hotkey : Attribute
    {
        public string? FuncToCall { get; set; }
        public string? JSFuncToCall { get; set; }
    }
}