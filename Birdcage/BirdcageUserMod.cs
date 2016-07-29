namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using ColossalFramework.Plugins;
    using ICities;
    using SexyFishHorse.CitiesSkylines.Birdcage.Options;
    using SexyFishHorse.CitiesSkylines.Infrastructure;
    using SexyFishHorse.CitiesSkylines.Infrastructure.Configuration;
    using SexyFishHorse.CitiesSkylines.Logger;

    public class BirdcageUserMod : IUserModWithOptionsPanel
    {
        private readonly OptionsUiController optionsUiController;

        private readonly ILogger logger;

        public BirdcageUserMod()
        {
            optionsUiController = new OptionsUiController(new ConfigStore("Birdcage"));

            logger = LogManager.Instance.GetOrCreateLogger("Birdcage");
            logger.Info("test");
        }

        public string Description
        {
            get
            {
                return "More Chirper controls";
            }
        }

        public string Name
        {
            get
            {
                return "Birdcage";
            }
        }

        public void OnSettingsUI(UIHelperBase uiHelper)
        {
            uiHelper.AddButton("test", () => { logger.Info("click click"); });
            logger.Info("on settings ui");
            try
            {
                optionsUiController.ConfigureUi(uiHelper);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }
    }
}
