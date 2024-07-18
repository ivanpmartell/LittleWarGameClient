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
                }
            };
            settings.SaveTo(fileName);
        }

        internal void SetMouseLock(bool value)
        {
            settings["Mouse"]["lock"] = value;
            settings.SaveTo(fileName);
        }

        internal bool GetMouseLock()
        {
            return settings["Mouse"]["lock"];
        }

        internal void SetFullScreen(bool value)
        {
            settings["Window"]["fullscreen"] = value;
            settings.SaveTo(fileName);
        }

        internal bool GetFullScreen()
        {
            return settings["Window"]["fullscreen"];
        }

        internal void SetWindowSize(int width, int height)
        {
            settings["Window"]["width"] = width;
            settings["Window"]["height"] = height;
            settings.SaveTo(fileName);
        }

        internal Size GetWindowSize()
        {
            int width = settings["Window"]["width"];
            int height = settings["Window"]["height"];
            return new Size(width, height);
        }
    }
}
