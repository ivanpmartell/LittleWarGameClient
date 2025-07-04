using IniFile;

namespace LittleWarGameClient.Helpers
{
    internal class SettingsHelper
    {
        private readonly Ini settings;
        internal SettingsHelper(Ini s)
        {
            settings = s;
        }

        internal static string EnumToCommaSeparatedString<T>() where T : Enum
        {
            return string.Join(",", Enum.GetValues(typeof(T)).OfType<T>());
        }

        internal OverlayType GetVariable(string sectionName, string propertyName, OverlayType defaultValue)
        {
            try
            {
                return settings[sectionName][propertyName].AsEnum<OverlayType>();
            }
            catch
            {
                return defaultValue;
            }
        }

        internal Keys GetVariable(string sectionName, string propertyName, Keys defaultValue)
        {
            try
            {
                return settings[sectionName][propertyName].AsEnum<Keys>();
            }
            catch
            {
                return defaultValue;
            }
        }

        internal bool GetVariable(string sectionName, string propertyName, bool defaultValue)
        {
            try
            {
                return settings[sectionName][propertyName];
            }
            catch
            {
                return defaultValue;
            }
        }

        internal int GetVariable(string sectionName, string propertyName, int defaultValue)
        {
            try
            {
                return settings[sectionName][propertyName];
            }
            catch
            {
                return defaultValue;
            }
        }

        internal double GetVariable(string sectionName, string propertyName, double defaultValue)
        {
            try
            {
                return settings[sectionName][propertyName];
            }
            catch
            {
                return defaultValue;
            }
        }

        internal string GetVariable(string sectionName, string propertyName, string defaultValue)
        {
            try
            {
                return settings[sectionName][propertyName];
            }
            catch
            {
                return defaultValue;
            }
        }

        internal DateTime GetVariable(string sectionName, string propertyName, DateTime defaultValue)
        {
            try
            {
                return settings[sectionName][propertyName];
            }
            catch
            {
                return defaultValue;
            }
        }

        internal Version GetVariable(string sectionName, string propertyName, Version defaultValue)
        {
            try
            {
                return new Version(settings[sectionName][propertyName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        private Section EnsureSection(string sectionName)
        {
            Section section = settings[sectionName];
            if (section == null)
            {
                section = new Section(sectionName);
                settings.Add(section);
            }
            return section;
        }

        internal void SetVariable(string sectionName, string propertyName, bool value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value;
            }
            catch
            {
                section.Add(new Property(propertyName, value));
            }
        }

        internal void SetVariable(string sectionName, string propertyName, DateTime value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value;
            }
            catch
            {
                section.Add(new Property(propertyName, value));
            }
        }

        internal void SetVariable(string sectionName, string propertyName, int value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value;
            }
            catch
            {
                section.Add(new Property(propertyName, value));
            }
        }

        internal void SetVariable(string sectionName, string propertyName, double value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value.ToString("0.##");
            }
            catch
            {
                section.Add(new Property(propertyName, value.ToString("0.##")));
            }
        }

        internal void SetVariable(string sectionName, string propertyName, string value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value;
            }
            catch
            {
                section.Add(new Property(propertyName, value));
            }
        }

        internal void SetVariable(string sectionName, string propertyName, Keys value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value.ToString();
            }
            catch
            {
                section.Add(new Property(propertyName, value.ToString()));
            }
        }

        internal void SetVariable(string sectionName, string propertyName, OverlayType value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value.ToString();
            }
            catch
            {
                section.Add(new Property(propertyName, value.ToString()));
            }
        }

        internal void SetVariable(string sectionName, string propertyName, Version value)
        {
            Section section = EnsureSection(sectionName);
            try
            {
                section[propertyName] = value.ToString();
            }
            catch
            {
                section.Add(new Property(propertyName, value.ToString()));
            }
        }
    }
}
