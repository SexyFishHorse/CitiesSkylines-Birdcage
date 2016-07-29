namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColossalFramework.Plugins;
    using ColossalFramework.UI;
    using ICities;
    using SexyFishHorse.CitiesSkylines.Infrastructure;
    using SexyFishHorse.CitiesSkylines.Infrastructure.Configuration;
    using SexyFishHorse.CitiesSkylines.Infrastructure.UI;
    using SexyFishHorse.CitiesSkylines.Logger;
    using UnityEngine;

    public class BirdcageUserMod : ChirperExtensionBase, IUserModWithOptionsPanel
    {
        private const string SettingKeysChirperAnchor = "ChirperAnchor";

        private const string SettingKeysChirperPositionX = "ChirperPositionX";

        private const string SettingKeysChirperPositionY = "ChirperPositionY";

        private const string SettingKeysDraggable = "Draggable";

        private const string SettingKeysFilterMessages = "FilterMessages";

        private const string SettingKeysHideChirper = "HideChirper";

        private readonly IConfigStore configStore;

        private readonly ILogger logger;

        private readonly HashSet<IChirperMessage> messagesToRemove;

        private bool controlDown;

        private bool draggable;

        private bool dragging;

        private bool filterNonImportantMessages;

        private bool mouseDown;

        private AudioClip notificationSound;

        private UIView uiView;

        private Vector2 defaultPosition;

        public BirdcageUserMod()
        {
            messagesToRemove = new HashSet<IChirperMessage>();
            configStore = new ConfigStore("Birdcage");

            logger = LogManager.Instance.GetOrCreateLogger("Birdcage");

            filterNonImportantMessages = configStore.GetSetting<bool>(SettingKeysFilterMessages);
            draggable = configStore.GetSetting<bool>(SettingKeysDraggable);
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

            defaultPosition = c.builtinChirperPosition;

            if (configStore.HasSetting(SettingKeysChirperPositionX))
            {
                var chirperX = configStore.GetSetting<int>(SettingKeysChirperPositionX);
                var chirperY = configStore.GetSetting<int>(SettingKeysChirperPositionY);
                var chirperPosition = new Vector2(chirperX, chirperY);
                chirperPosition = EnsurePositionIsOnScreen(chirperPosition);

                var chirperAnchor = (ChirperAnchor)configStore.GetSetting<int>(SettingKeysChirperAnchor);

                chirper.builtinChirperPosition = chirperPosition;
                chirper.SetBuiltinChirperAnchor(chirperAnchor);
            }
            else
            {
                chirper.builtinChirperPosition = new Vector2(100, 100);
                chirper.SetBuiltinChirperAnchor(ChirperAnchor.TopLeft);
            }

            if (draggable)
            {
                c.SetBuiltinChirperFree(true);
            }

            var hideChirper = configStore.GetSetting<bool>(SettingKeysHideChirper);

            notificationSound = ChirpPanel.instance.m_NotificationSound;
            ChirpPanel.instance.gameObject.SetActive(!hideChirper);
            uiView = ChirpPanel.instance.component.GetUIView();
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
                    configStore.GetSetting<bool>(SettingKeysHideChirper),
                    ToggleChirper);
                appearanceGroup.AddCheckBox(
                    "Make Chirper draggable (hold ctrl + left mouse button)",
                    configStore.GetSetting<bool>(SettingKeysDraggable),
                    ToggleDraggable);
                appearanceGroup.AddButton("Reset Chirper position", ResetPosition);

                var behaviourGroup = helper.AddGroup("Behaviour");

                behaviourGroup.AddCheckBox(
                    "Filter non-important messages",
                    configStore.GetSetting<bool>(SettingKeysFilterMessages),
                    ToggleFilter);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }

        private void ResetPosition()
        {
            chirper.builtinChirperPosition = defaultPosition;
            chirper.SetBuiltinChirperAnchor(ChirperAnchor.TopCenter);

            SaveChirperPosition(ChirperAnchor.TopCenter);
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
                    ChirpPanel.instance.Collapse();
                    foreach (var chirperMessage in messagesToRemove)
                    {
                        MessageManager.instance.DeleteMessage(chirperMessage);
                    }

                    ChirpPanel.instance.SynchronizeMessages();
                    ChirpPanel.instance.m_NotificationSound = notificationSound;
                    messagesToRemove.Clear();
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
                        var mousePosition = GetMouseGuiPosition();

                        mousePosition = EnsurePositionIsOnScreen(mousePosition);

                        chirper.builtinChirperPosition = mousePosition;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }

        private Vector2 EnsurePositionIsOnScreen(Vector2 mousePosition)
        {
            if (mousePosition.x < 0)
            {
                mousePosition.x = 0;
            }

            if (mousePosition.x > uiView.GetScreenResolution().x)
            {
                mousePosition.x = uiView.GetScreenResolution().x;
            }

            if (mousePosition.y < 0)
            {
                mousePosition.y = 0;
            }

            if (mousePosition.y > uiView.GetScreenResolution().y)
            {
                mousePosition.y = uiView.GetScreenResolution().y;
            }
            return mousePosition;
        }

        private Vector2 GetMouseGuiPosition()
        {
            var mouseWorldPos = Camera.current.ScreenToWorldPoint(Input.mousePosition);
            var mouseGuiPos = uiView.WorldPointToGUI(uiView.uiCamera, mouseWorldPos);
            return mouseGuiPos;
        }

        private bool IsMouseOnChirper()
        {
            var mouseGuiPos = GetMouseGuiPosition();

            var pointAndChirperMagnitude = mouseGuiPos - chirper.builtinChirperPosition;

            var mouseOnChirper = pointAndChirperMagnitude.magnitude < 35;
            return mouseOnChirper;
        }

        private void SaveChirperPositionAndUpdateAnchor()
        {
            var thirdOfUiWidth = uiView.GetScreenResolution().x / 3;
            var halfUiHeight = uiView.GetScreenResolution().y / 2;

            ChirperAnchor anchor;

            if (chirper.builtinChirperPosition.x < thirdOfUiWidth)
            {
                anchor = chirper.builtinChirperPosition.y < halfUiHeight
                             ? ChirperAnchor.TopLeft
                             : ChirperAnchor.BottomLeft;
            }
            else if (chirper.builtinChirperPosition.x > thirdOfUiWidth * 2)
            {
                anchor = chirper.builtinChirperPosition.y < halfUiHeight
                             ? ChirperAnchor.TopRight
                             : ChirperAnchor.BottomRight;
            }
            else if (chirper.builtinChirperPosition.y < halfUiHeight)
            {
                anchor = ChirperAnchor.TopCenter;
            }
            else
            {
                anchor = ChirperAnchor.BottomCenter;
            }

            chirper.SetBuiltinChirperAnchor(anchor);

            SaveChirperPosition(anchor);
        }

        private void SaveChirperPosition(ChirperAnchor anchor)
        {
            configStore.SaveSetting(SettingKeysChirperPositionX, (int)chirper.builtinChirperPosition.x);
            configStore.SaveSetting(SettingKeysChirperPositionY, (int)chirper.builtinChirperPosition.y);
            configStore.SaveSetting(SettingKeysChirperAnchor, (int)anchor);
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
                configStore.SaveSetting(SettingKeysHideChirper, hideChirper);

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

                configStore.SaveSetting(SettingKeysFilterMessages, shouldFilter);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
        }
    }
}
