namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System;
    using ICities;
    using Infrastructure;
    using Logger;
    using UnityEngine;
    using ILogger = Logger.ILogger;

    public class BirdcageUserMod : UserModBase, IChirperExtension
    {
        public const string ModName = "Birdcage";

        private readonly FilterService filterService;

        private readonly InputService inputService;

        private readonly ILogger logger;

        private readonly PositionService positionService;

        private bool dragging;

        private bool initialized;

        private IChirper chirper;

        public BirdcageUserMod()
        {
            logger = LogManager.Instance.GetOrCreateLogger(ModName);

            filterService = new FilterService();
            inputService = new InputService();
            positionService = new PositionService();

            OptionsPanelManager = new OptionsPanelManager(logger, positionService);
        }

        public override string Description
        {
            get
            {
                return "More Chirper controls";
            }
        }

        public override string Name
        {
            get
            {
                return ModName;
            }
        }

        public AudioClip NotificationSound { get; set; }

        public void OnMessagesUpdated()
        {
        }

        public void OnNewMessage(IChirperMessage message)
        {
            if (ModConfig.Instance.GetSetting<bool>(SettingKeys.FilterMessages))
            {
                filterService.HandleNewMessage(message);
            }
        }

        public void OnCreated(IChirper chirper)
        {
            this.chirper = chirper;
            ((OptionsPanelManager)OptionsPanelManager).Chirper = chirper;
        }

        public void OnReleased()
        {
        }

        public void OnUpdate()
        {
            Initialize();

            try
            {
                if (ChirpPanel.instance == null)
                {
                    return;
                }

                if (ModConfig.Instance.GetSetting<bool>(SettingKeys.FilterMessages))
                {
                    filterService.RemovePendingMessages(NotificationSound);
                }

                if (ModConfig.Instance.GetSetting<bool>(SettingKeys.Draggable))
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

            if (ModConfig.Instance.GetSetting<bool>(SettingKeys.Draggable))
            {
                chirper.SetBuiltinChirperFree(true);

                if (ModConfig.Instance.GetSetting<int>(SettingKeys.ChirperPositionX) > 0)
                {
                    var chirperX = ModConfig.Instance.GetSetting<int>(SettingKeys.ChirperPositionX);
                    var chirperY = ModConfig.Instance.GetSetting<int>(SettingKeys.ChirperPositionY);
                    var chirperPosition = new Vector2(chirperX, chirperY);

                    positionService.UpdateChirperPosition(chirperPosition);
                }
            }

            var hideChirper = ModConfig.Instance.GetSetting<bool>(SettingKeys.HideChirper);
            chirper.ShowBuiltinChirper(!hideChirper);

            initialized = true;
        }

        private void StopDragging()
        {
            positionService.UpdateChirperAnchor();
            positionService.SaveChirperPosition();
        }
    }
}
