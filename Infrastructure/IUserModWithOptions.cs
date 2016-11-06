namespace SexyFishHorse.CitiesSkylines.Infrastructure
{
    using ICities;
    using JetBrains.Annotations;

    /// <summary>
    /// This interface inherits from <see cref="IUserMod"/> and includes the method signature for the settings UI.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IUserModWithOptionsPanel : IUserMod
    {
        // ReSharper disable once InconsistentNaming
        void OnSettingsUI(UIHelperBase uiHelperBase);
    }
}
