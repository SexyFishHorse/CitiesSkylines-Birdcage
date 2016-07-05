namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using ICities;

    public interface IOptionsController
    {
        T GetSetting<T>(string settingName);

        OnCheckChanged HideChirper();
    }
}
