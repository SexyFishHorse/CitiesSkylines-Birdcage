namespace SexyFishHorse.CitiesSkylines.Infrastructure.IO
{
    using System;
    using System.IO;
    using System.Linq;
    using JetBrains.Annotations;

    public static class CitiesSkylinesPaths
    {
        public static string GetModFolderPath([NotNull] string modFolderName, ModFolder modFolder)
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var colossalOrderPath = Path.Combine(localAppDataPath, "Colossal Order");
            var citiesSkylinesPath = Path.Combine(colossalOrderPath, "Cities_Skylines");
            var modFolderPath = Path.Combine(citiesSkylinesPath, modFolderName);

            var attribute =
                modFolder.GetType()
                         .GetCustomAttributes(typeof(ModFolderPathAttribute), false)
                         .Cast<ModFolderPathAttribute>()
                         .Single();

            return attribute.Paths.Aggregate(modFolderPath, Path.Combine);
        }
    }
}
