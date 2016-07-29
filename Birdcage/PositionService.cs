namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using ColossalFramework.UI;
    using ICities;
    using UnityEngine;

    public class PositionService
    {
        public IChirper Chirper { get; set; }

        public Vector2 DefaultPosition { get; set; }

        public UIView UiView { get; set; }

        public ChirperAnchor CalculateChirperAnchor()
        {
            var thirdOfUiWidth = UiView.GetScreenResolution().x / 3;
            var halfUiHeight = UiView.GetScreenResolution().y / 2;

            ChirperAnchor anchor;

            if (Chirper.builtinChirperPosition.x < thirdOfUiWidth)
            {
                anchor = Chirper.builtinChirperPosition.y < halfUiHeight
                             ? ChirperAnchor.TopLeft
                             : ChirperAnchor.BottomLeft;
            }
            else if (Chirper.builtinChirperPosition.x > thirdOfUiWidth * 2)
            {
                anchor = Chirper.builtinChirperPosition.y < halfUiHeight
                             ? ChirperAnchor.TopRight
                             : ChirperAnchor.BottomRight;
            }
            else if (Chirper.builtinChirperPosition.y < halfUiHeight)
            {
                anchor = ChirperAnchor.TopCenter;
            }
            else
            {
                anchor = ChirperAnchor.BottomCenter;
            }

            return anchor;
        }

        public Vector2 EnsurePositionIsOnScreen(Vector2 mousePosition)
        {
            if (mousePosition.x < 25)
            {
                mousePosition.x = 25;
            }

            if (mousePosition.x > UiView.GetScreenResolution().x - 40)
            {
                mousePosition.x = UiView.GetScreenResolution().x - 40;
            }

            if (mousePosition.y < 30)
            {
                mousePosition.y = 30;
            }

            if (mousePosition.y > UiView.GetScreenResolution().y - 150)
            {
                mousePosition.y = UiView.GetScreenResolution().y - 150;
            }

            return mousePosition;
        }

        public Vector2 GetMouseGuiPosition()
        {
            var mouseWorldPos = Camera.current.ScreenToWorldPoint(Input.mousePosition);
            var mouseGuiPos = UiView.WorldPointToGUI(UiView.uiCamera, mouseWorldPos);

            return mouseGuiPos;
        }
    }
}
