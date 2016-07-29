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
        private const string SettingKeysFilterMessages = "FilterMessages";

        private const string SettingKeysHideChirper = "HideChirper";

        private readonly IConfigStore configStore;

        private readonly ILogger logger;

        private readonly HashSet<IChirperMessage> messagesToRemove;

        private bool filterNonImportantMessages;

        private AudioClip notificationSound;

        public BirdcageUserMod()
        {
            messagesToRemove = new HashSet<IChirperMessage>();
            configStore = new ConfigStore("Birdcage");

            logger = LogManager.Instance.GetOrCreateLogger("Birdcage");

            filterNonImportantMessages = configStore.GetSetting<bool>(SettingKeysFilterMessages);
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

            var hideChirper = configStore.GetSetting<bool>(SettingKeysHideChirper);

            notificationSound = ChirpPanel.instance.m_NotificationSound;
            ChirpPanel.instance.gameObject.SetActive(!hideChirper);
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

                helper.AddCheckBox("Hide chirper", configStore.GetSetting<bool>(SettingKeysHideChirper), ToggleChirper);
                helper.AddCheckBox(
                    "Filter non-important messages", 
                    configStore.GetSetting<bool>(SettingKeysFilterMessages), 
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
            }
            catch (Exception ex)
            {
                logger.LogException(ex, PluginManager.MessageType.Error);
            }
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
