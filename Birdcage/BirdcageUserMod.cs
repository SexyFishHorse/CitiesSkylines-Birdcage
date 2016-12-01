namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using ICities;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Infrastructure.UI;
    using Logger;
    using UnityEngine;
    using ILogger = Logger.ILogger;

    public class BirdcageUserMod : ChirperExtensionBase, IUserModWithOptionsPanel<BirdcageUserMod>, IUserMod
    {
        private const string ModName = "Birdcage";

        private readonly IConfigStore configStore;

        private readonly FilterService filterService;

        private readonly InputService inputService;

        private readonly ILogger logger;

        private readonly PositionService positionService;

        private bool draggable;

        private bool dragging;

        private bool filterNonImportantMessages;

        private bool initialized;

        public BirdcageUserMod()
        {
            configStore = new ConfigStore(ModName);
            filterService = new FilterService();
            inputService = new InputService();
            positionService = new PositionService();

            logger = LogManager.Instance.GetOrCreateLogger(ModName);

            filterNonImportantMessages = configStore.GetSetting<bool>(SettingKeys.FilterMessages);
            draggable = configStore.GetSetting<bool>(SettingKeys.Draggable);
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
                return ModName;
            }
        }

        public AudioClip NotificationSound { get; set; }

        public override void OnNewMessage(IChirperMessage message)
        {
            if (filterNonImportantMessages)
            {
                filterService.HandleNewMessage(message);
            }
        }

        public void OnSettingsUI(UIHelperBase uiHelperBase)
        {
            try
            {
                var uiHelper = uiHelperBase.AsStronglyTyped();

                var appearanceGroup = uiHelper.AddGroup("Appearance");
                var behaviourGroup = uiHelper.AddGroup("Behaviour");

                appearanceGroup.AddCheckBox(
                    "Hide chirper",
                    configStore.GetSetting<bool>(SettingKeys.HideChirper),
                    ToggleChirper);
                appearanceGroup.AddCheckBox(
                    "Make Chirper draggable (hold ctrl + left mouse button)",
                    configStore.GetSetting<bool>(SettingKeys.Draggable),
                    ToggleDraggable);
                appearanceGroup.AddButton("Reset Chirper position", ResetPosition);

                behaviourGroup.AddCheckBox(
                    "Filter non-important messages",
                    configStore.GetSetting<bool>(SettingKeys.FilterMessages),
                    ToggleFilter);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        public override void OnUpdate()
        {
            Initialize();

            try
            {
                if (ChirpPanel.instance == null)
                {
                    return;
                }

                if (filterNonImportantMessages)
                {
                    filterService.RemovePendingMessages(NotificationSound);
                }

                if (draggable)
                {
                    inputService.SetPrimaryMouseButtonDownState();
                    inputService.SetAnyControlDownState();

                    if (dragging && !inputService.PrimaryMouseButtonDownState)
                    {
                        dragging = false;
                        StopDragging();
                    }
                    else if (!dragging && inputService.PrimaryMouseButtonDownState && inputService.AnyControlDown)
                    {
                        if (positionService.IsMouseOnChirper())
                        {
                            dragging = true;
                        }
                    }
                    else if (dragging)
                    {
                        positionService.Dragging();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        // TODO: Move this to OnCreated when CO fixes the initial state position mismatch
        private void Initialize()
        {
            if (initialized)
            {
                return;
            }

            positionService.Chirper = chirper;
            positionService.DefaultPosition = chirper.builtinChirperPosition;
            positionService.UiView = ChirpPanel.instance.component.GetUIView();

            NotificationSound = ChirpPanel.instance.m_NotificationSound;

            if (draggable)
            {
                chirper.SetBuiltinChirperFree(true);

                if (configStore.HasSetting(SettingKeys.ChirperPositionX))
                {
                    var chirperX = configStore.GetSetting<int>(SettingKeys.ChirperPositionX);
                    var chirperY = configStore.GetSetting<int>(SettingKeys.ChirperPositionY);
                    var chirperPosition = new Vector2(chirperX, chirperY);

                    positionService.UpdateChirperPosition(chirperPosition);
                }
            }

            var hideChirper = configStore.GetSetting<bool>(SettingKeys.HideChirper);
            chirper.ShowBuiltinChirper(!hideChirper);

            initialized = true;
        }

        private void ResetPosition()
        {
            if (chirper == null)
            {
                return;
            }

            positionService.ResetPosition();

            SaveChirperPosition();
        }

        private void SaveChirperPosition()
        {
            configStore.SaveSetting(SettingKeys.ChirperPositionX, (int) chirper.builtinChirperPosition.x);
            configStore.SaveSetting(SettingKeys.ChirperPositionY, (int) chirper.builtinChirperPosition.y);
        }

        private void StopDragging()
        {
            positionService.UpdateChirperAnchor();
            SaveChirperPosition();
        }

        private void ToggleChirper(bool hideChirper)
        {
            try
            {
                configStore.SaveSetting(SettingKeys.HideChirper, hideChirper);

                if (ChirpPanel.instance != null)
                {
                    ChirpPanel.instance.gameObject.SetActive(!hideChirper);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void ToggleDraggable(bool isDraggable)
        {
            draggable = isDraggable;
            if (chirper != null)
            {
                chirper.SetBuiltinChirperFree(true);
            }

            configStore.SaveSetting(SettingKeys.Draggable, isDraggable);
        }

        private void ToggleFilter(bool shouldFilter)
        {
            try
            {
                filterNonImportantMessages = shouldFilter;

                configStore.SaveSetting(SettingKeys.FilterMessages, shouldFilter);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
    }
}
