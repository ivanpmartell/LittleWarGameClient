using IniFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleWarGameClient
{
    internal class Settings
    {
        private const string fileName = "Settings.ini";
        private readonly Ini settings;
        public Settings()
        {
            if (!File.Exists(fileName))
                CreateDefaultIniFile();
            settings = new Ini(fileName);
        }

        private void CreateDefaultIniFile()
        {
            var settings = new Ini
            {
                new Section("Window")
                {
                    new Property("width", 1280),
                    new Property("height", 720),
                    new Property("fullscreen", false)
                },
                new Section("Mouse")
                {
                    new Property("lock", false)
                },
                new Section("Update")
                {
                    new Property("lastChecked", DateTime.MinValue),
                    new Property("interval", 7),
                }
            };
            settings.SaveTo(fileName);
        }

        internal async void SetMouseLock(bool value)
        {
            await Task.Run(() =>
            {
                settings["Mouse"]["lock"] = value;
                settings.SaveTo(fileName);
            });
        }

        internal bool GetMouseLock()
        {
            return settings["Mouse"]["lock"];
        }

        internal async void SetFullScreen(bool value)
        {
            await Task.Run(() =>
            {
                settings["Window"]["fullscreen"] = value;
                settings.SaveTo(fileName);
            });
        }

        internal bool GetFullScreen()
        {
            return settings["Window"]["fullscreen"];
        }

        internal async void SetWindowSize(int width, int height)
        {
            await Task.Run(() =>
            {
                settings["Window"]["width"] = width;
                settings["Window"]["height"] = height;
                settings.SaveTo(fileName);
            });
        }

        internal Size GetWindowSize()
        {
            int width = settings["Window"]["width"];
            int height = settings["Window"]["height"];
            return new Size(width, height);
        }

        internal async void SetLastUpdated(DateTime value)
        {
            await Task.Run(() =>
            {
                settings["Update"]["lastChecked"] = value;
                settings.SaveTo(fileName);
            });
        }

        internal DateTime GetLastUpdated()
        {
            return settings["Update"]["lastChecked"];
        }

        internal async void SetUpdateInterval(int value)
        {
            await Task.Run(() =>
            {
                settings["Update"]["interval"] = value;
                settings.SaveTo(fileName);
            });
        }

        internal int GetUpdateInterval()
        {
            return settings["Update"]["interval"];
        }
    }
}
