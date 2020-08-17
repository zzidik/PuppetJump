using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.SteamVR
{

    public class InputDevice_SteamVR : InputDevice
    {
        private int deviceIndex;            // holds the index of the input device
        [System.Serializable]
        public class InputEvents
        {
            public TriggerEvents trigger;
            public TriggerEvents grip;
            public JoystickEvents thumbPad;
        }
        public InputEvents inputEvents;

        void Update()
        {
            // need to constanlty monitior which controller is which with SteamVR
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (PuppetJumpManager.Instance.rightHandVRInputDevice != PuppetJumpManager.Instance.cameraRig.GetComponent<SteamVR_ControllerManager>().right.GetComponent<InputDevice>())
            {
                PuppetJumpManager.Instance.rightHandVRInputDevice = PuppetJumpManager.Instance.cameraRig.GetComponent<SteamVR_ControllerManager>().right.GetComponent<InputDevice>();
            }

            if (PuppetJumpManager.Instance.leftHandVRInputDevice != PuppetJumpManager.Instance.cameraRig.GetComponent<SteamVR_ControllerManager>().left.GetComponent<InputDevice>())
            {
                PuppetJumpManager.Instance.leftHandVRInputDevice = PuppetJumpManager.Instance.cameraRig.GetComponent<SteamVR_ControllerManager>().left.GetComponent<InputDevice>();
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            //constantly update the index of this device cause it might change
            deviceIndex = GetDeviceIndex();

            // trigger down
            if (SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                inputEvents.trigger.down.Invoke();
                inputEvents.trigger.isDown = true;
            }

            // trigger up
            if (SteamVR_Controller.Input(deviceIndex).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                inputEvents.trigger.up.Invoke();
                inputEvents.trigger.isDown = false;
            }

            // grip down
            if (SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                inputEvents.grip.down.Invoke();
                inputEvents.grip.isDown = true;
            }

            // grip up
            if (SteamVR_Controller.Input(deviceIndex).GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                inputEvents.grip.up.Invoke();
                inputEvents.grip.isDown = false;
            }

            // touchpad down
            if (SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                inputEvents.thumbPad.down.Invoke();
                inputEvents.thumbPad.isDown = true;
            }

            // touchpad up
            if (SteamVR_Controller.Input(deviceIndex).GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                inputEvents.thumbPad.up.Invoke();
                inputEvents.thumbPad.isDown = false;
            }

            // touchpad position
            inputEvents.thumbPad.position = new Vector2(SteamVR_Controller.Input(deviceIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x, SteamVR_Controller.Input(deviceIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y);
            inputEvents.thumbPad.positionUpdate.Invoke(inputEvents.thumbPad.position);
        }

        public int GetDeviceIndex()
        {
            int deviceIndex = -1;
            if (GetComponent<SteamVR_TrackedObject>())
            {
                deviceIndex = (int)GetComponent<SteamVR_TrackedObject>().index;
            }
            return deviceIndex;
        }
    }
}
