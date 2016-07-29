namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using ColossalFramework.Plugins;
    using ICities;
    using SexyFishHorse.CitiesSkylines.Infrastructure;
    using SexyFishHorse.CitiesSkylines.Infrastructure.Configuration;
    using SexyFishHorse.CitiesSkylines.Infrastructure.UI;
    using SexyFishHorse.CitiesSkylines.Logger;

    public class BirdcageUserMod : ChirperExtensionBase, IUserModWithOptionsPanel
    {
        private const string SettingKeysHideChirper = "HideChirper";

        private readonly IConfigStore configStore;

        private readonly ILogger logger;

        public BirdcageUserMod()
        {
            configStore = new ConfigStore("Birdcage");

            logger = LogManager.Instance.GetOrCreateLogger("Birdcage");
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

        public override void OnCreated(IChirper c)
        {
            base.OnCreated(c);

            var hideChirper = configStore.GetSetting<bool>(SettingKeysHideChirper);

            ChirpPanel.instance.gameObject.SetActive(!hideChirper);
        }

        public void OnSettingsUI(UIHelperBase uiHelper)
        {
            try
            {
                var helper = uiHelper.AsStronglyTyped();

                helper.AddCheckBox("Hide chirper", configStore.GetSetting<bool>(SettingKeysHideChirper), ToggleChirper);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }

        private void ToggleChirper(bool hideChirper)
        {
            try
            {
                configStore.SaveSetting(SettingKeysHideChirper, hideChirper);

                if (ChirpPanel.instance != null)
                {
                    ChirpPanel.instance.gameObject.SetActive(!hideChirper);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }
    }
}
