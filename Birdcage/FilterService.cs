namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using System.Collections.Generic;
    using System.Linq;
    using ICities;
    using UnityEngine;

    public class FilterService
    {
        private readonly HashSet<IChirperMessage> messagesToRemove;

        public FilterService()
        {
            messagesToRemove = new HashSet<IChirperMessage>();
        }

        public void HandleNewMessage(IChirperMessage message)
        {
            var citizenMessage = message as CitizenMessage;
            if (citizenMessage == null)
            {
                return;
            }

            if (!ShouldFilterMessage(citizenMessage))
            {
                return;
            }

            messagesToRemove.Add(message);
            ChirpPanel.instance.m_NotificationSound = null;
        }

        public void RemovePendingMessages(AudioClip notificationSound)
        {
            if (!messagesToRemove.Any())
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

        private bool ShouldFilterMessage(CitizenMessage citizenMessage)
        {
            switch (citizenMessage.m_messageID)
            {
                case LocaleID.CHIRP_ABANDONED_BUILDINGS:
                case LocaleID.CHIRP_COMMERCIAL_DEMAND:
                case LocaleID.CHIRP_DEAD_PILING_UP:
                case LocaleID.CHIRP_DISASTER:
                case LocaleID.CHIRP_FIRE_HAZARD:
                case LocaleID.CHIRP_HIGH_CRIME:
                case LocaleID.CHIRP_INDUSTRIAL_DEMAND:
                case LocaleID.CHIRP_LOW_HAPPINESS:
                case LocaleID.CHIRP_LOW_HEALTH:
                case LocaleID.CHIRP_MILESTONE_REACHED:
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
                case LocaleID.EDITORCHIRPER_REQUIREMENTS:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_FERTILITY:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_FOREST:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_OIL:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_ORE:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_PLANE:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_RECOMMENDED:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_ROADIN:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_ROADOUT:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_SHIP:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_TRAIN:
                case LocaleID.EDITORCHIRPER_REQUIREMENTS_WATER:
                case LocaleID.FOOTBALLCHIRP_LOSE:
                case LocaleID.FOOTBALLCHIRP_WIN:
                    return false;
                default:
                    return true;
            }
        }
    }
}
