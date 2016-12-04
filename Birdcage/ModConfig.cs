namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using Infrastructure.Configuration;

    public static class ModConfig
    {
        private static ConfigurationManager instance;

        public static ConfigurationManager Instance
        {
            get
            {
                return instance ?? (instance = ConfigurationManagerFactory.GetOrCreateConfigurationManager(BirdcageUserMod.ModName));
            }
        }
    }
}
