namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using ICities;
    using SexyFishHorse.CitiesSkylines.Infrastructure;

    public class BirdcageUserMod : IUserModWithOptionsPanel
    {
        public string Name
        {
            get { return "Birdcage"; }
        }

        public string Description
        {
            get { return "More Chirper controls"; }
        }

        public void OnSettingsUI(UIHelperBase uiHelper)
        {
            throw new NotImplementedException();
        }
    }
}
