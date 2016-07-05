namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using ColossalFramework.UI;
    using ICities;
    using SexyFishHorse.CitiesSkylines.Infrastructure;

    public class BirdcageUserMod : IUserModWithOptionsPanel
    {
        private readonly IOptionsController optionsController;

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
                    optionsController.GetSetting<bool>("HideChirper"), 
                    optionsController.HideChirper());
        }
    }
}
