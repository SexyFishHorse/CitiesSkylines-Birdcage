namespace SexyFishHorse.CitiesSkylines.Infrastructure
{
    using ICities;
    using JetBrains.Annotations;

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IUserModWithOptionsPanel : IUserMod
    {
        // ReSharper disable once InconsistentNaming
        void OnSettingsUI(UIHelperBase uiHelperBase);
    }
}
