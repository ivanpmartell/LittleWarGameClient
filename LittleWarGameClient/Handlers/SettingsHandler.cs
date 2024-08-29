using IniFile;
using LittleWarGameClient.Helpers;

namespace LittleWarGameClient.Handlers
{
    internal class SettingsHandler
    {
        private const string fileName = "Settings.ini";
        private const int defaultWidth = 1280;
        private const int defaultHeight = 720;
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

        public SettingsHandler()
        {
            if (!File.Exists(fileName))
                settings = CreateDefaultIniFile();
            else
                settings = new Ini(fileName);
            helper = new SettingsHelper(settings);
            Init();
        }

        private async void Init()
        {
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
                },
                new Section("Mouse")
                {
                    new Property("lock", defaultMouseLock)
                },
                new Section("Update")
                {
                    new Property("lastChecked", defaultUpdateLastChecked),
                    new Property("interval", defaultUpdateInterval),
                },
                new Section("Hotkeys")
                {
                    new Property("optionsMenu", defaultOptionsMenuHotkey.ToString()),
                    new Property("friendsMenu", defaultFriendsMenuHotkey.ToString()),
                    new Property("chatHistoryMenu", defaultChatHistoryMenuHotkey.ToString()),
                    new Property("fullscreen", defaultFullscreenHotkey.ToString())
                },
                new Section("Audio")
                {
                    new Property("volume", defaultVolume)
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
                using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    await settings.SaveToAsync(stream);
                }
            }
            catch
            {
                Thread.Sleep(50);
                await SaveAsync();
            }
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
    }

    internal class Hotkey : Attribute
    {
        public string? FuncToCall { get; set; }
        public string? JSFuncToCall { get; set; }
    }
}