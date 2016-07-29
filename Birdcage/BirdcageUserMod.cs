namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using ColossalFramework.Plugins;
    using ICities;
    using SexyFishHorse.CitiesSkylines.Infrastructure;
    using SexyFishHorse.CitiesSkylines.Infrastructure.Configuration;
    using SexyFishHorse.CitiesSkylines.Infrastructure.UI;
    using SexyFishHorse.CitiesSkylines.Logger;
    using UnityEngine;

    public class BirdcageUserMod : ChirperExtensionBase, IUserModWithOptionsPanel
    {
        private readonly IConfigStore configStore;

        private readonly FilterService filterService;

        private readonly ILogger logger;

        private readonly PositionService positionService;

        private bool controlDown;

        private bool draggable;

        private bool dragging;

        private bool filterNonImportantMessages;

        private bool mouseDown;

        public BirdcageUserMod()
        {
            configStore = new ConfigStore("Birdcage");
            filterService = new FilterService();
            positionService = new PositionService();

            logger = LogManager.Instance.GetOrCreateLogger("Birdcage");

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
                return "Birdcage";
            }
        }

        public AudioClip NotificationSound { get; set; }

        public override void OnCreated(IChirper c)
        {
            base.OnCreated(c);

            positionService.Chirper = c;
            positionService.UiView = ChirpPanel.instance.component.GetUIView();
            positionService.DefaultPosition = chirper.builtinChirperPosition;

            NotificationSound = ChirpPanel.instance.m_NotificationSound;

            if (draggable)
            {
                c.SetBuiltinChirperFree(true);

                if (configStore.HasSetting(SettingKeys.ChirperPositionX))
                {
                    var chirperX = configStore.GetSetting<int>(SettingKeys.ChirperPositionX);
                    var chirperY = configStore.GetSetting<int>(SettingKeys.ChirperPositionY);
                    var chirperPosition = new Vector2(chirperX, chirperY);
                    chirperPosition = positionService.EnsurePositionIsOnScreen(chirperPosition);

                    var chirperAnchor = (ChirperAnchor)configStore.GetSetting<int>(SettingKeys.ChirperAnchor);

                    chirper.builtinChirperPosition = chirperPosition;
                    chirper.SetBuiltinChirperAnchor(chirperAnchor);
                }
            }

            var hideChirper = configStore.GetSetting<bool>(SettingKeys.HideChirper);
            chirper.ShowBuiltinChirper(!hideChirper);
        }

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
                var helper = uiHelper.AsStronglyTyped();

                var appearanceGroup = helper.AddGroup("Appearance");

                appearanceGroup.AddCheckBox(
                    "Hide chirper",
                    configStore.GetSetting<bool>(SettingKeys.HideChirper),
                    ToggleChirper);
                appearanceGroup.AddCheckBox(
                    "Make Chirper draggable (hold ctrl + left mouse button)",
                    configStore.GetSetting<bool>(SettingKeys.Draggable),
                    ToggleDraggable);
                appearanceGroup.AddButton("Reset Chirper position", ResetPosition);

                var behaviourGroup = helper.AddGroup("Behaviour");

                behaviourGroup.AddCheckBox(
                    "Filter non-important messages",
                    configStore.GetSetting<bool>(SettingKeys.FilterMessages),
                    ToggleFilter);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }

        public override void OnUpdate()
        {
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
                    if (Input.GetMouseButtonDown(0))
                    {
                        mouseDown = true;
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        mouseDown = false;
                    }

                    if (dragging && mouseDown == false)
                    {
                        dragging = false;
                        SaveChirperPositionAndUpdateAnchor();
                    }

                    if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
                    {
                        controlDown = true;
                    }

                    if ((Input.GetKeyUp(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                        || (Input.GetKeyUp(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftCommand)))
                    {
                        controlDown = false;
                    }

                    if (!dragging && mouseDown && controlDown)
                    {
                        if (IsMouseOnChirper())
                        {
                            dragging = true;
                        }
                    }

                    if (dragging)
                    {
                        ChirpPanel.instance.Collapse();
                        var mousePosition = positionService.GetMouseGuiPosition();

                        mousePosition = positionService.EnsurePositionIsOnScreen(mousePosition);

                        chirper.builtinChirperPosition = mousePosition;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }

        private bool IsMouseOnChirper()
        {
            var mouseGuiPos = positionService.GetMouseGuiPosition();

            var pointAndChirperMagnitude = mouseGuiPos - chirper.builtinChirperPosition;

            var mouseOnChirper = pointAndChirperMagnitude.magnitude < 35;
            return mouseOnChirper;
        }

        private void ResetPosition()
        {
            const ChirperAnchor Anchor = ChirperAnchor.TopCenter;

            chirper.builtinChirperPosition = positionService.DefaultPosition;
            chirper.SetBuiltinChirperAnchor(Anchor);

            SaveChirperPosition(Anchor);
        }

        private void SaveChirperPosition(ChirperAnchor anchor)
        {
            configStore.SaveSetting(SettingKeys.ChirperPositionX, (int)chirper.builtinChirperPosition.x);
            configStore.SaveSetting(SettingKeys.ChirperPositionY, (int)chirper.builtinChirperPosition.y);
            configStore.SaveSetting(SettingKeys.ChirperAnchor, (int)anchor);
        }

        private void SaveChirperPositionAndUpdateAnchor()
        {
            var anchor = positionService.CalculateChirperAnchor();

            chirper.SetBuiltinChirperAnchor(anchor);
            SaveChirperPosition(anchor);
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
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }

        private void ToggleDraggable(bool isDraggable)
        {
            draggable = isDraggable;
            if (chirper != null)
            {
                chirper.SetBuiltinChirperFree(true);
            }
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
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }
    }
}
