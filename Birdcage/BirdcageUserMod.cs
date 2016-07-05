namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using ColossalFramework.UI;
    using ICities;
    using SexyFishHorse.CitiesSkylines.Infrastructure;
    using SexyFishHorse.CitiesSkylines.Infrastructure.Configuration;

    public class BirdcageUserMod : IUserModWithOptionsPanel
    {
        private readonly IOptionsController optionsController;

        private readonly ConfigStore configStore;

        public BirdcageUserMod()
        {
            optionsController = new OptionsController();
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
            var hideChirperButton =
                (UIButton)
                uiHelper.AddCheckbox(
                    "Hide chirper", 
                    configStore.GetSetting<bool>("HideChirper"), 
                    optionsController.HideChirper());
        }
    }
}
