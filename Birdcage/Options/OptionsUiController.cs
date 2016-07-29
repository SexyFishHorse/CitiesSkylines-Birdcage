namespace SexyFishHorse.CitiesSkylines.Birdcage.Options
{
    using ICities;
    using SexyFishHorse.CitiesSkylines.Infrastructure.Configuration;
    using SexyFishHorse.CitiesSkylines.Infrastructure.UI;

    public class OptionsUiController
    {
        private const string HideChirperConfigKey = "HideChirper";

        private readonly IConfigStore configStore;

        public OptionsUiController(IConfigStore configStore)
        {
            this.configStore = configStore;
        }

        public void ConfigureUi(UIHelperBase uiHelperBase)
        {
            var uiHelper = uiHelperBase.AsStronglyTyped();

            uiHelper.AddCheckBox("Hide chirper", configStore.GetSetting<bool>(HideChirperConfigKey), ToggleChirper);
        }

        private void ToggleChirper(bool isChecked)
        {
            configStore.SaveSetting(HideChirperConfigKey, isChecked);
        }
    }
}
