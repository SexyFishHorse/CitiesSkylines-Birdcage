namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        private readonly ILogger logger;

        private readonly HashSet<IChirperMessage> messagesToRemove;

        private readonly PositionService positionService;

        private bool controlDown;

        private bool draggable;

        private bool dragging;

        private bool filterNonImportantMessages;

        private bool mouseDown;

        private AudioClip notificationSound;

        public BirdcageUserMod()
        {
            messagesToRemove = new HashSet<IChirperMessage>();
            configStore = new ConfigStore("Birdcage");
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

        public override void OnCreated(IChirper c)
        {
            base.OnCreated(c);

            positionService.Chirper = c;
            positionService.UiView = ChirpPanel.instance.component.GetUIView();
            positionService.DefaultPosition = chirper.builtinChirperPosition;

            notificationSound = ChirpPanel.instance.m_NotificationSound;

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
            if (!filterNonImportantMessages)
            {
                return;
            }

            var citizenMessage = message as CitizenMessage;
            if (citizenMessage == null)
            {
                return;
            }

            if (ShouldFilterMessage(citizenMessage))
            {
                ChirpPanel.instance.m_NotificationSound = null;

                messagesToRemove.Add(message);
            }
        }

        public void OnSettingsUI(UIHelperBase uiHelper)
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

                if (messagesToRemove.Any())
                {
                    RemoveMessages();
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

        private void RemoveMessages()
        {
            ChirpPanel.instance.Collapse();
            foreach (var chirperMessage in messagesToRemove)
            {
                MessageManager.instance.DeleteMessage(chirperMessage);
            }

            ChirpPanel.instance.SynchronizeMessages();
            ChirpPanel.instance.m_NotificationSound = notificationSound;
            messagesToRemove.Clear();
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

        private bool ShouldFilterMessage(CitizenMessage citizenMessage)
        {
            switch (citizenMessage.m_messageID)
            {
                case LocaleID.CHIRP_ABANDONED_BUILDINGS:
                case LocaleID.CHIRP_COMMERCIAL_DEMAND:
                case LocaleID.CHIRP_DEAD_PILING_UP:
                case LocaleID.CHIRP_FIRE_HAZARD:
                case LocaleID.CHIRP_HIGH_CRIME:
                case LocaleID.CHIRP_INDUSTRIAL_DEMAND:
                case LocaleID.CHIRP_LOW_HAPPINESS:
                case LocaleID.CHIRP_LOW_HEALTH:
                case LocaleID.CHIRP_NEED_MORE_PARKS:
                case LocaleID.CHIRP_NEW_MAP_TILE:
                case LocaleID.CHIRP_NOISEPOLLUTION:
                case LocaleID.CHIRP_NO_ELECTRICITY:
                case LocaleID.CHIRP_NO_HEALTHCARE:
                case LocaleID.CHIRP_NO_PRISONS:
                case LocaleID.CHIRP_NO_SCHOOLS:
                case LocaleID.CHIRP_NO_WATER:
                case LocaleID.CHIRP_POISONED:
                case LocaleID.CHIRP_POLLUTION:
                case LocaleID.CHIRP_RESIDENTIAL_DEMAND:
                case LocaleID.CHIRP_SEWAGE:
                case LocaleID.CHIRP_TRASH_PILING_UP:
                    return false;
                default:
                    return true;
            }
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
