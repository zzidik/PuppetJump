using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.Destop
{
    public class InputDevice_Mouse : InputDevice
    {
        [System.Serializable]
        public class InputEvents
        {
            public Vector2Event mousePos;
            public ButtonEvents mouseButtonLeft;
        }
        public InputEvents inputEvents;

        private void Update()
        {
            inputEvents.mousePos.Invoke(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

            if (Input.GetMouseButtonDown(0))
            {
                inputEvents.mouseButtonLeft.down.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                inputEvents.mouseButtonLeft.up.Invoke();
            }


        }
    }
}
