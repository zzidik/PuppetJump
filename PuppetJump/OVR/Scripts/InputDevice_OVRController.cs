using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.OVR
{
    public class InputDevice_OVRController : InputDevice
    {
        public OVRInput.Controller OVRController;   // identifies the OVR controller 

        [System.Serializable]
        public class InputEvents
        {
            public TriggerEvents trigger;
            public TriggerEvents grip;
            public JoystickEvents joystick;
            public ButtonEvents buttonOne;
            public ButtonEvents buttonTwo;
        }
        public InputEvents inputEvents;

        void Update()
        {
            // trigger down
            // for oculus touch and tracked remotes
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRController))
            {
                inputEvents.trigger.down.Invoke();
                inputEvents.trigger.isDown = true;
            }

            // trigger up
            // for oculus touch and tracked remotes
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRController))
            {
                inputEvents.trigger.up.Invoke();
                inputEvents.trigger.isDown = false;
            }

            // trigger position (range 0.0f - 1.0f)
            inputEvents.trigger.position = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRController);

            // grip down
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRController))
            {
                inputEvents.grip.down.Invoke();
                inputEvents.grip.isDown = true;
            }

            // grip up
            if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRController))
            {
                inputEvents.grip.up.Invoke();
                inputEvents.grip.isDown = false;
            }

            // grip position (range 0.0f - 1.0f)
            inputEvents.grip.position = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRController);

            // thumbStick down
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRController))
            {
                inputEvents.joystick.down.Invoke();
                inputEvents.joystick.isDown = true;
            }

            // thumbStick up
            if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRController))
            {
                inputEvents.joystick.up.Invoke();
                inputEvents.joystick.isDown = false;
            }

            // thumbStick position (x,y range -1.0f - 1.0f)
            inputEvents.joystick.position = new Vector2(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRController).x, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRController).y);
            inputEvents.joystick.positionUpdate.Invoke(inputEvents.joystick.position);

            // buttonOne down (A on Right, X on Left Controller)
            if (OVRInput.GetDown(OVRInput.Button.One, OVRController))
            {
                inputEvents.buttonOne.down.Invoke();
                inputEvents.buttonOne.isDown.Invoke();
            }

            // buttonOne up (A on Right, X on Left Controller)
            if (OVRInput.GetUp(OVRInput.Button.One, OVRController))
            {
                inputEvents.buttonOne.up.Invoke();
                inputEvents.buttonOne.isDown.Invoke();
            }

            // buttonTwo down (B on Right, Y on Left Controller)
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRController))
            {
                inputEvents.buttonTwo.down.Invoke();
                inputEvents.buttonTwo.isDown.Invoke();
            }

            // buttonTwo up (B on Right, Y on Left Controller)
            if (OVRInput.GetUp(OVRInput.Button.Two, OVRController))
            {
                inputEvents.buttonTwo.up.Invoke();
                inputEvents.buttonTwo.isDown.Invoke();
            }
        }
    }
}
