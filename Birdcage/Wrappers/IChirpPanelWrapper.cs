namespace SexyFishHorse.CitiesSkylines.Birdcage.Wrappers
{
    using UnityEngine;

    public interface IChirpPanelWrapper
    {
        void CollapsePanel();

        void RemoveNotificationSound();

        void SetNotificationSound(AudioClip notificationSound);

        void SynchronizeMessages();
    }
}
