namespace SexyFishHorse.CitiesSkylines.Birdcage.Wrappers
{
    using UnityEngine;

    public class ChirpPanelWrapper : IChirpPanelWrapper
    {
        public void CollapsePanel()
        {
            ChirperUtils.CollapseChirperInstantly();
        }

        public void RemoveNotificationSound()
        {
            ChirpPanel.instance.m_NotificationSound = null;
        }

        public void SetNotificationSound(AudioClip notificationSound)
        {
            ChirpPanel.instance.m_NotificationSound = notificationSound;
        }

        public void SynchronizeMessages()
        {
            ChirpPanel.instance.SynchronizeMessages();
        }
    }
}
