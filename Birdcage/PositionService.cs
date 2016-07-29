namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using ColossalFramework.UI;
    using ICities;
    using UnityEngine;

    public class PositionService
    {
        private const int ChirperBottomBound = 160;

        private const int ChirperLeftBound = 25;

        private const int ChirperPositionMagnitude = 35;

        private const int ChirperRightBound = 40;

        private const int ChirperTopBound = 30;

        public IChirper Chirper { get; set; }

        public Vector2 DefaultPosition { get; set; }

        public UIView UiView { get; set; }

        public void Dragging()
        {
            ChirpPanel.instance.Collapse();
            var mousePosition = GetMouseGuiPosition();

            mousePosition = EnsurePositionIsOnScreen(mousePosition);

            Chirper.builtinChirperPosition = mousePosition;
        }

        public bool IsMouseOnChirper()
        {
            var mouseGuiPos = GetMouseGuiPosition();

            var pointAndChirperMagnitude = mouseGuiPos - Chirper.builtinChirperPosition;

            return pointAndChirperMagnitude.magnitude < ChirperPositionMagnitude;
        }

        public void ResetPosition()
        {
            Chirper.builtinChirperPosition = DefaultPosition;
            Chirper.SetBuiltinChirperAnchor(ChirperAnchor.TopCenter);
        }

        public void UpdateChirperAnchor()
        {
            Chirper.SetBuiltinChirperAnchor(CalculateAnchor());
        }

        public void UpdateChirperPosition(Vector2 chirperPosition)
        {
            chirperPosition = EnsurePositionIsOnScreen(chirperPosition);

            Chirper.builtinChirperPosition = chirperPosition;
            UpdateChirperAnchor();
        }

        private ChirperAnchor CalculateAnchor()
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

        private Vector2 EnsurePositionIsOnScreen(Vector2 mousePosition)
        {
            if (mousePosition.x < ChirperLeftBound)
            {
                mousePosition.x = ChirperLeftBound;
            }

            if (mousePosition.x > UiView.GetScreenResolution().x - ChirperRightBound)
            {
                mousePosition.x = UiView.GetScreenResolution().x - ChirperRightBound;
            }

            if (mousePosition.y < ChirperTopBound)
            {
                mousePosition.y = ChirperTopBound;
            }

            if (mousePosition.y > UiView.GetScreenResolution().y - ChirperBottomBound)
            {
                mousePosition.y = UiView.GetScreenResolution().y - ChirperBottomBound;
            }

            return mousePosition;
        }

        private Vector2 GetMouseGuiPosition()
        {
            var mouseWorldPos = Camera.current.ScreenToWorldPoint(Input.mousePosition);
            var mouseGuiPos = UiView.WorldPointToGUI(UiView.uiCamera, mouseWorldPos);

            return mouseGuiPos;
        }
    }
}
