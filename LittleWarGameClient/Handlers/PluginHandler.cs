using LittleWarGameClient.Helpers;
using Octokit;
using SharpCompress.Common.Tar;
using SharpCompress.Readers.Tar;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LittleWarGameClient.Handlers
{
    internal class PluginHandler
    {
        private readonly SettingsHandler settings;
        private readonly string pluginsDirectory;
        private readonly string enabledFilePath;
        private readonly string localReleaseFile;
        private readonly Dictionary<string, Plugin> installedPlugins;
        private readonly Dictionary<string, Version> latestPluginVersions;
        private DateTime latestGithubCall = DateTime.Now.AddMinutes(-5);

        internal PluginHandler(string exeDirectory, SettingsHandler s)
        {
            settings = s;
            pluginsDirectory = Path.Combine(exeDirectory, "plugins");
            enabledFilePath = Path.Combine(pluginsDirectory, "enabled.txt");
            localReleaseFile = Path.Combine(pluginsDirectory, "*.tar.gz");
            installedPlugins = ObtainInstalledPlugins();
            latestPluginVersions = GetLatestVersionsOfAvailablePlugins();
        }

        internal void SynchronizeWithLocalPlugins()
        {
            ObtainInstalledPlugins();
        }

        internal List<string> GetEnabledPluginIdsThatModifyLoadingImage()
        {
            List<string> result = new();
            foreach (var (pluginId, plugin) in installedPlugins)
            {
                if (plugin.Enabled)
                {
                    if (plugin.ModifiesLoadingImage())
                        result.Add(pluginId);
                }
            }
            return result;
        }

        internal List<string> GetEnabledPluginIdsThatModifyGameScript()
        {
            List<string> result = new();
            foreach (var (pluginId, plugin) in installedPlugins)
            {
                if (plugin.Enabled)
                {
                    if (plugin.ModifiesGameScript())
                        result.Add(pluginId);
                }
            }
            return result;
        }

        internal void EnableAPlugin(string id)
        {
            if (installedPlugins.ContainsKey(id))
            {
                if (!installedPlugins[id].Enabled)
                {
                    installedPlugins[id].Enabled = true;
                    SynchronizeEnabledPluginsWithFile();
                }
            }
        }

        internal void DisableAPlugin(string id)
        {
            if (installedPlugins.ContainsKey(id))
            {
                if (installedPlugins[id].Enabled)
                {
                    installedPlugins[id].Enabled = false;
                    SynchronizeEnabledPluginsWithFile();
                }
            }
        }

        internal bool InstallOrUpdateAPlugin(string id)
        {
            var result = Task.Run(() => InstallPluginAsync(id)).GetAwaiter().GetResult();
            if (result != null)
            {
                if (installedPlugins.ContainsKey(id))
                    installedPlugins.Remove(id);
                installedPlugins.Add(id, new Plugin(result));
                return true;
            }
            return false;
        }

        internal Dictionary<string, Version> GetLatestVersionsOfAvailablePlugins()
        {
            Dictionary<string, Version> latestVersions = new();
            var availablePlugins = GetAvailablePluginsOnline();
            foreach (var (pluginId, plugin) in availablePlugins)
            {
                latestVersions.Add(pluginId, plugin.GetVersion());
            }
            return latestVersions;
        }

        internal void UninstallAPlugin(string id)
        {
            installedPlugins.Remove(id);
            string pluginPath = Path.Combine(pluginsDirectory, id);
            if (Directory.Exists(pluginPath))
                Directory.Delete(pluginPath, true);
        }

        internal Dictionary<string, Plugin> GetAvailablePluginsOnline()
        {
            return Task.Run(() => GetAvailablePluginsOnlineAsync()).GetAwaiter().GetResult();
        }

        internal Dictionary<string, Plugin> GetInstalledPlugins()
        {
            return installedPlugins;
        }

        private Dictionary<string, Plugin> ObtainInstalledPlugins()
        {
            string[] pluginDirs = Directory.GetDirectories(pluginsDirectory);
            Dictionary<string, Plugin> plugins = new();
            foreach (string pluginDir in pluginDirs)
            {
                AddPluginToList(plugins, pluginDir);
            }
            SetEnabledPlugins(plugins);
            return plugins;
        }

        private async Task<string?> InstallPluginAsync(string id)
        {
            string pluginPath = Path.Combine(pluginsDirectory, id);
            string? localReleaseFile = await LatestPluginsReleaseFileDownloaded();
            if (localReleaseFile != null)
            {
                using (Stream stream = File.OpenRead(localReleaseFile))
                {
                    var reader = TarReader.Open(stream);
                    while (reader.MoveToNextEntry())
                    {
                        TarEntry entry = reader.Entry;
                        if (entry.Key == null)
                            continue;
                        if (entry.IsDirectory && Path.GetFileName(Path.TrimEndingDirectorySeparator(entry.Key)) == id)
                        {
                            if (Directory.Exists(pluginPath))
                                Directory.Delete(pluginPath, true);
                            Directory.CreateDirectory(pluginPath);
                        }
                        else if (Path.GetFileName(Path.GetDirectoryName(entry.Key)) == id)
                        {
                            using (var entryStream = reader.OpenEntryStream())
                            {
                                if (entryStream != null)
                                {
                                    using (var fileStream = File.Create(Path.Combine(pluginPath, Path.GetFileName(entry.Key))))
                                    {
                                        await entryStream.CopyToAsync(fileStream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (Directory.Exists(pluginPath))
                return pluginPath;
            return null;
        }

        private void SynchronizeEnabledPluginsWithFile()
        {
            using (StreamWriter writer = new(enabledFilePath))
            {
                foreach (var (pluginId, plugin) in installedPlugins)
                {
                    if (plugin.Enabled)
                    {
                        writer.WriteLine(pluginId);
                    }
                }
            }
        }

        private void SetEnabledPlugins(Dictionary<string, Plugin> installedPlugins)
        {
            if (!File.Exists(enabledFilePath))
            {
                using (StreamWriter writer = new(enabledFilePath))
                {
                    writer.Write("sample");
                }
            }
            string tempFile = Path.GetTempFileName();
            using (StreamReader reader = new(enabledFilePath))
            {
                using (StreamWriter writer = new(tempFile))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string pluginFolder = Path.Combine(pluginsDirectory, line);
                        if (installedPlugins.ContainsKey(line))
                        {
                            installedPlugins[line].Enabled = true;
                            writer.WriteLine(line);
                        }
                    }
                }
            }
            File.Delete(enabledFilePath);
            File.Move(tempFile, enabledFilePath);
        }

        private async Task<Dictionary<string, Plugin>> GetAvailablePluginsOnlineAsync()
        {
            string? localReleaseFile = await LatestPluginsReleaseFileDownloaded();
            if (localReleaseFile != null)
                return GetPluginsFromTarball(localReleaseFile);
            else
                return new Dictionary<string, Plugin>();
        }

        private async Task<string?> LatestPluginsReleaseFileDownloaded()
        {
            Release release = await GetLatestPluginsReleaseTag();
            var localReleaseFile = Path.Combine(pluginsDirectory, $"plugins.tar.gz");
            if (release.TagName != null)
            {
                var match = Regex.Match(release.TagName, @"\d+(\.\d+)+");
                if (match.Success)
                {
                    var latestOnlineReleaseVersion = new Version(match.Value);
                    if (settings.GetPluginRepoReleaseVersion() < latestOnlineReleaseVersion)
                    {
                        await DownloadReleaseTarball(release.TarballUrl, localReleaseFile);
                        if (!IOHelper.IsTarFile(localReleaseFile))
                            return null;
                        settings.SetPluginRepoReleaseVersion(latestOnlineReleaseVersion);
                    }
                    return localReleaseFile;
                }
            }
            else if (File.Exists(localReleaseFile))
            {
                return localReleaseFile;
            }
            return null;
        }

        private async Task DownloadReleaseTarball(string url, string savePath)
        {
            using (HttpClient httpClient = new())
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LWGClient/1.0 (Windows NT 10.0; Win64; x64)");
                var result = await httpClient.GetAsync(url);
                var fileInfo = new FileInfo(savePath);
                using (var fileStream = fileInfo.Create())
                {
                    await result.Content.CopyToAsync(fileStream);
                }
            }
        }

        private async Task<Release> GetLatestPluginsReleaseTag()
        {
            if (DateTime.Now <= latestGithubCall.AddMinutes(5))
                return new Release();
            try
            {
                latestGithubCall = DateTime.Now;
                var client = new GitHubClient(new ProductHeaderValue("LWGClient"));
                return await client.Repository.Release.GetLatest("ivanpmartell", "LittleWarGamePlugins");
            }
            catch(RateLimitExceededException)
            {
                return new Release();
            }
        }

        private Dictionary<string, Plugin> GetPluginsFromTarball(string filePath)
        {
            var plugins = new Dictionary<string, Plugin>();
            using (Stream stream = File.OpenRead(filePath))
            {
                var reader = TarReader.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    TarEntry entry = reader.Entry;
                    if (entry.Key == null)
                        continue;
                    if (entry.Key.EndsWith("plugin.txt"))
                    {
                        string folder = Path.GetFileName(Path.GetDirectoryName(entry.Key)!);
                        using (StreamReader entryReader = new(reader.OpenEntryStream()))
                        {
                            AddPluginToList(plugins, entryReader, folder);
                        }
                    }
                }
            }
            return plugins;
        }

        private void AddPluginToList(Dictionary<string, Plugin> plugins, string pluginDirectory)
        {
            try
            {
                Plugin plugin = new(pluginDirectory);
                plugins.Add(plugin.Folder, plugin);
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(ex.Message, "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Plugin Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void AddPluginToList(Dictionary<string, Plugin> plugins, StreamReader reader, string folder)
        {
            try
            {
                plugins.Add(folder, new Plugin(reader, folder));
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(ex.Message, "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }

    public class Plugin
    {
        [Required]
        public string Name { get; private set; }

        [Required]
        public string Description { get; private set; }

        [Required]
        public string Version { get; private set; }

        [Ignore]
        public string Folder { get; private set; } //This is the plugin Id

        [Ignore]
        internal string? AbsolutePluginPath { get; private set; }

        [Ignore]
        public bool Enabled { get; internal set; } = false;

        [Ignore]
        internal Size? ImageSize
        {
            get
            {
                if (Image_width != null && Image_height != null)
                    return new Size((int)Image_width, (int)Image_height);
                else
                    return null;
            }
            private set { }
        }

        [Ignore]
        internal Point? ImageLocation
        {
            get
            {
                if (Image_x != null && Image_y != null)
                    return new Point((int) Image_x, (int) Image_y);
                else
                    return null;
            }
            private set { }
        }

        private int? Image_x { get; set; }
        private int? Image_y { get; set; }
        private int? Image_width { get; set; }
        private int? Image_height { get; set; }

        internal string? Image { get; private set; }

        internal string? GameScript { get; private set; }

        internal List<string> Scripts { get; private set; }

#pragma warning disable CS8618
        internal Plugin(string path)
#pragma warning restore CS8618
        {
            AbsolutePluginPath = path;
            Folder = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
            string pluginFile = Path.Combine(path, "plugin.txt");
            if (!File.Exists(pluginFile))
            {
                throw new FileNotFoundException($"The plugin information file (plugin.txt) does not exist in folder: {Folder}");
            }
            using (StreamReader reader = new(pluginFile))
            {
                PopulateProperties(ParseContents(reader));
            }
        }

#pragma warning disable CS8618
        internal Plugin(StreamReader reader, string folder)
#pragma warning restore CS8618
        {
            Folder = folder;
            PopulateProperties(ParseContents(reader));
        }

        internal Version GetVersion()
        {
            return new Version(Version);
        }

        internal bool ModifiesGameScript()
        {
            return GameScript != null;
        }

        internal bool ModifiesLoadingImage()
        {
            return Image != null || ImageSize != null || ImageLocation != null;
        }

        private Dictionary<string, string> ParseContents(StreamReader reader)
        {
            Dictionary<string, string> variables = new();
            string? line;
            int lineNum = 0;
            string currentVariable = "";
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                lineNum++;
                if (line.StartsWith("#") || String.IsNullOrEmpty(line))
                    continue;
                if (line.Contains("::"))
                {
                    string[] parts = line.Split("::");
                    currentVariable = parts[0].Trim().ToLower();
                    switch (currentVariable)
                    {
                        case "name":
                        case "description":
                        case "version":
                        case "image_x":
                        case "image_y":
                        case "image_width":
                        case "image_height":
                        case "image":
                        case "gamescript":
                        case "scripts":
                            variables[currentVariable] = parts[1].Trim();
                            continue;
                        default:
                            throw new NotSupportedException($"Unknown property in plugin.txt at line: {lineNum}");
                    }
                }
                else
                {
                    if (currentVariable == "")
                    {
                        throw new NotSupportedException($"Empty property name in plugin.txt at line: {lineNum}");
                    }
                    variables[currentVariable] += "\n" + line;
                }
            }
            return variables;
        }

        private void PopulateProperties(Dictionary<string, string> variables)
        {
            Type type = typeof(Plugin);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (Attribute.IsDefined(property, typeof(IgnoreAttribute)))
                    continue;
                var lowercasePropertyName = property.Name.ToLower();
                if (variables.ContainsKey(lowercasePropertyName))
                {
                    string varValue = variables[lowercasePropertyName];
                    if (IsGenericList(property.PropertyType))
                    {
                        string[] scriptsArray = varValue.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                        Type ListObjectsType = property.PropertyType.GetGenericArguments()[0];
                        var IListRef = typeof(List<>);
                        Type[] IListParam = { ListObjectsType };
                        object? scriptsList = Activator.CreateInstance(IListRef.MakeGenericType(IListParam));
                        if (scriptsList != null)
                        {
                            foreach (var token in scriptsArray)
                            {
                                scriptsList.GetType().GetMethod("Add")!.Invoke(scriptsList, new[] { Convert.ChangeType(token.Trim(), ListObjectsType) });
                            }
                            property.SetValue(this, scriptsList);
                        }
                    }
                    else if (property.PropertyType != varValue.GetType())
                    {
                        if (IsNullable(property.PropertyType))
                        {
                            Type NullableObjectsType = property.PropertyType.GetGenericArguments()[0];
                            property.SetValue(this, Convert.ChangeType(varValue.Replace("\n", ""), NullableObjectsType));
                        }
                        else
                            property.SetValue(this, Convert.ChangeType(varValue.Replace("\n", ""), property.PropertyType));
                    }
                    else
                        property.SetValue(this, varValue);
                }
                else
                {
                    if (property.GetCustomAttribute(typeof(RequiredAttribute)) != null)
                        throw new NotSupportedException($"Missing a required variable in plugin.txt: {property.Name}");
                    else
                        property.SetValue(this, default);
                }

            }
        }

        private static bool IsGenericList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        private static bool IsNullable(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}
