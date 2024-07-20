using IniFile;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LittleWarGameClient
{
    internal class Settings
    {
        private const string fileName = "Settings.ini";
        private const int defaultWidth = 1280;
        private const int defaultHeight = 720;
        private const bool defaultFullscreen = false;
        private const bool defaultMouseLock = false;
        private const int defaultUpdateInterval = 1;
        private readonly DateTime defaultUpdateLastChecked = DateTime.MinValue;
        private const Keys defaultOptionsMenu = Keys.F10;
        private const Keys defaultFriendsMenu = Keys.F9;
        private const Keys defaultChatHistoryMenu = Keys.F11;
        private const Keys defaultFullscreenHotkey = Keys.F8;
        private readonly Ini settings;
        private readonly SettingsHelper helper;

        public Settings()
        {
            if (!File.Exists(fileName))
                CreateDefaultIniFile();
            settings = new Ini(fileName);
            helper = new SettingsHelper(settings);
            Init();
        }

        private void Init()
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
            Save();
        }

        private void CreateDefaultIniFile()
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
                    new Property("optionsMenu", defaultOptionsMenu.ToString()),
                    new Property("friendsMenu", defaultFriendsMenu.ToString()),
                    new Property("chatHistoryMenu", defaultChatHistoryMenu.ToString()),
                    new Property("fullscreen", defaultFullscreenHotkey.ToString())
                }
            };
            settings.SaveTo(fileName);
        }

        internal void Save()
        {
            settings.SaveTo(fileName);
        }

        internal async void SaveAsync()
        {
            await Task.Run(() => {
                for (int numTries = 0; numTries < 5; numTries++)
                {
                    try
                    {
                        settings.SaveTo(fileName);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(50);
                    }
                }
            });
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
            return helper.GetVariable("Hotkeys", "optionsMenu", defaultOptionsMenu);
        }

        internal void SetFriendsMenuHotkey(Keys value)
        {
            helper.SetVariable("Hotkeys", "friendsMenu", value);
        }

        [Hotkey(FuncToCall = "FriendsHotkeyFunc", JSFuncToCall = "addFriendsMenuHotkey")]
        public Keys GetFriendsMenuHotkey()
        {
            return helper.GetVariable("Hotkeys", "friendsMenu", defaultFriendsMenu);
        }

        internal void SetChatHistoryMenuHotkey(Keys value)
        {
            helper.SetVariable("Hotkeys", "chatHistoryMenu", value);
        }

        [Hotkey(FuncToCall = "ChatHistoryHotkeyFunc", JSFuncToCall = "addChatHistoryHotkey")]
        public Keys GetChatHistoryMenuHotkey()
        {
            return helper.GetVariable("Hotkeys", "chatHistoryMenu", defaultChatHistoryMenu);
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
    }

    internal class Hotkey : Attribute
    {
        public string? FuncToCall { get; set; }
        public string? JSFuncToCall { get; set; }
    }
}