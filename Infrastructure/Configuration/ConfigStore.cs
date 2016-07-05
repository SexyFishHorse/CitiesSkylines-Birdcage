namespace SexyFishHorse.CitiesSkylines.Infrastructure.Configuration
{
    using System;
    using System.IO;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using SexyFishHorse.CitiesSkylines.Infrastructure.IO;

    public class ConfigStore : IConfigStore
    {
        public const string ConfigFileName = "ModConfiguration.json";

        public ConfigStore(string modFolderName)
        {
            var modFolderPath = GamePaths.GetModFolderPath(GameFolder.Mods);
            modFolderPath = Path.Combine(modFolderPath, modFolderName);

            Directory.CreateDirectory(modFolderPath);

            ConfigFileInfo = new FileInfo(Path.Combine(modFolderPath, ConfigFileName));
        }

        public FileInfo ConfigFileInfo { get; private set; }

        public T GetSetting<T>(string key)
        {
            EnforceKeyIsValid(key);

            var modConfiguration = LoadConfigFromFile();

            object value;
            if (modConfiguration.Settings.TryGetValue(key, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        public void RemoveSetting(string key)
        {
            EnforceKeyIsValid(key);

            var config = LoadConfigFromFile();

            config.Settings.Remove(key);

            SaveConfigToFile(config);
        }

        public void SaveSetting<T>(string key, T value)
        {
            EnforceKeyIsValid(key);

            if (value == null)
            {
                throw new ArgumentNullException(
                    "value", 
                    string.Format("The configuration value for the key {0} is null. Use RemoveSetting to remove a value", key));
            }

            var modConfiguration = LoadConfigFromFile();

            modConfiguration.Settings.Add(key, value);

            SaveConfigToFile(modConfiguration);
        }

        [AssertionMethod]
        private static void EnforceKeyIsValid([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("The config key is invalid", "key");
            }
        }

        private ModConfiguration LoadConfigFromFile()
        {
            if (!ConfigFileInfo.Exists)
            {
                return new ModConfiguration();
            }

            using (var fileStream = ConfigFileInfo.OpenRead())
            using (var streamReader = new StreamReader(fileStream))
            {
                var fileContent = streamReader.ReadToEnd();

                return JsonConvert.DeserializeObject<ModConfiguration>(fileContent);
            }
        }

        private void SaveConfigToFile(ModConfiguration modConfiguration)
        {
            using (var fileStream = ConfigFileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;

                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, modConfiguration);
            }
        }
    }
}
