namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System.Collections.Generic;
    using System.Linq;
    using ICities;
    using UnityEngine;

    public class FilterService
    {
        private static readonly HashSet<string> AllowedMessages = new HashSet<string>
        {
            LocaleID.CHIRP_ABANDONED_BUILDINGS,
            LocaleID.CHIRP_COMMERCIAL_DEMAND,
            LocaleID.CHIRP_DEAD_PILING_UP,
            LocaleID.CHIRP_DISASTER,
            LocaleID.CHIRP_FIRE_HAZARD,
            LocaleID.CHIRP_HIGH_CRIME,
            LocaleID.CHIRP_INDUSTRIAL_DEMAND,
            LocaleID.CHIRP_LOW_HAPPINESS,
            LocaleID.CHIRP_LOW_HEALTH,
            LocaleID.CHIRP_MILESTONE_REACHED,
            LocaleID.CHIRP_NEED_MORE_PARKS,
            LocaleID.CHIRP_NEW_MAP_TILE,
            LocaleID.CHIRP_NOISEPOLLUTION,
            LocaleID.CHIRP_NO_ELECTRICITY,
            LocaleID.CHIRP_NO_HEALTHCARE,
            LocaleID.CHIRP_NO_PRISONS,
            LocaleID.CHIRP_NO_SCHOOLS,
            LocaleID.CHIRP_NO_WATER,
            LocaleID.CHIRP_POISONED,
            LocaleID.CHIRP_POLLUTION,
            LocaleID.CHIRP_RESIDENTIAL_DEMAND,
            LocaleID.CHIRP_SEWAGE,
            LocaleID.CHIRP_TRASH_PILING_UP,
            LocaleID.EDITORCHIRPER_REQUIREMENTS,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_FERTILITY,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_FOREST,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_OIL,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_ORE,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_PLANE,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_RECOMMENDED,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_ROADIN,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_ROADOUT,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_SHIP,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_TRAIN,
            LocaleID.EDITORCHIRPER_REQUIREMENTS_WATER,
            LocaleID.FOOTBALLCHIRP_LOSE,
            LocaleID.FOOTBALLCHIRP_WIN,
        };

        private readonly HashSet<IChirperMessage> messagesToRemove = new HashSet<IChirperMessage>();

        public void HandleNewMessage(IChirperMessage message)
        {
            var citizenMessage = message as CitizenMessage;
            if (citizenMessage == null)
            {
                return;
            }

            if (AllowedMessages.Contains(citizenMessage.m_messageID))
            {
                return;
            }

            messagesToRemove.Add(message);
            ChirpPanel.instance.m_NotificationSound = null;
        }

        public void RemovePendingMessages(AudioClip notificationSound)
        {
            if (messagesToRemove.Any() == false)
            {
                return;
            }

            ChirperUtils.CollapseChirperInstantly();

            foreach (var chirperMessage in messagesToRemove)
            {
                MessageManager.instance.DeleteMessage(chirperMessage);
            }

            ChirpPanel.instance.SynchronizeMessages();
            ChirpPanel.instance.m_NotificationSound = notificationSound;
            messagesToRemove.Clear();
        }
    }
}
