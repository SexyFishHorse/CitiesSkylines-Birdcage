namespace SexyFishHorse.CitiesSkylines.Birdcage
{
    using UnityEngine;

    public class InputService
    {
        public bool AnyControlDown { get; private set; }

        public bool PrimaryMouseButtonDownState { get; private set; }

        public void SetAnyControlDownState()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                AnyControlDown = true;
            }
            else if ((Input.GetKeyUp(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                     || (Input.GetKeyUp(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftCommand)))
            {
                AnyControlDown = false;
            }
        }

        public void SetPrimaryMouseButtonDownState()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PrimaryMouseButtonDownState = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                PrimaryMouseButtonDownState = false;
            }
        }
    }
}
