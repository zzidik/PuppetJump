using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.Destop
{
    public class InputDevice_Keyboard : InputDevice
    {
        [System.Serializable]
        public class InputEvents
        {
            public ButtonEvents upArrow;
            public ButtonEvents downArrow;
            public ButtonEvents leftArrow;
            public ButtonEvents rightArrow;
            public ButtonEvents v;
        }
        public InputEvents inputEvents;

        private void Update()
        {
            if (Input.GetKeyDown("up"))
            {
                inputEvents.upArrow.down.Invoke();
            }

            if (Input.GetKeyUp("up"))
            {
                inputEvents.upArrow.up.Invoke();
            }

            if (Input.GetKey("up"))
            {
                inputEvents.upArrow.isDown.Invoke();
            }

            if (Input.GetKeyDown("down"))
            {
                inputEvents.downArrow.down.Invoke();
            }

            if (Input.GetKeyUp("down"))
            {
                inputEvents.downArrow.up.Invoke();
            }

            if (Input.GetKey("down"))
            {
                inputEvents.downArrow.isDown.Invoke();
            }

            if (Input.GetKeyDown("left"))
            {
                inputEvents.leftArrow.down.Invoke();
            }

            if (Input.GetKeyUp("left"))
            {
                inputEvents.leftArrow.up.Invoke();
            }

            if (Input.GetKey("left"))
            {
                inputEvents.leftArrow.isDown.Invoke();
            }

            if (Input.GetKeyDown("right"))
            {
                inputEvents.rightArrow.down.Invoke();
            }

            if (Input.GetKeyUp("right"))
            {
                inputEvents.rightArrow.up.Invoke();
            }

            if (Input.GetKey("right"))
            {
                inputEvents.rightArrow.isDown.Invoke();
            }

            if (Input.GetKeyDown("v"))
            {
                inputEvents.v.down.Invoke();
            }

            if (Input.GetKeyUp("v"))
            {
                inputEvents.v.up.Invoke();
            }

            if (Input.GetKey("v"))
            {
                inputEvents.v.isDown.Invoke();
            }
        }
    }
}
