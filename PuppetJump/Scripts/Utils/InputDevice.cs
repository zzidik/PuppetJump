using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PuppetJump.Utils
{
    public class InputDevice : MonoBehaviour
    {
        public enum DeviceType {  other, leftVRController, rightVRController };
        public DeviceType deviceType;

        /// <summary>
        /// A group of variables for a Button event.
        /// </summary>
        [System.Serializable]
        public class ButtonEvents
        {
            public UnityEvent down = new UnityEvent();          // triggers event(s) if button was pressed this frame
            public UnityEvent up = new UnityEvent();            // trigger event(s) if button was released this frame
            public UnityEvent isDown = new UnityEvent();
        }

        /// <summary>
        /// An event that requires the passing of a Vector2.
        /// </summary>

        [System.Serializable]
        public class Vector2Event : UnityEvent<Vector2> { };

        /// <summary>
        /// An event that requires the passing of a Vector3.
        /// </summary>

        [System.Serializable]
        public class Vector3Event : UnityEvent<Vector3> { };


        /// <summary>
        /// An event that requires the passing of a Quaternion.
        /// </summary>

        [System.Serializable]
        public class QuaternionEvent : UnityEvent<Quaternion> { };

        /// <summary>
        /// A group of events related to the transform of the input device.
        /// </summary>
        [System.Serializable]
        public class TransformEvents
        {
            public Vector3Event positionUpdate;
            public Vector3Event localPositionUpdate;
            public QuaternionEvent rotationUpdate;
            public QuaternionEvent localRotationUpdate;
        }


        /// <summary>
        /// A group of variables for Joystick events.
        /// </summary>
        [System.Serializable]
        public class JoystickEvents
        {
            public Vector2 position;                            // x and y ranging from -1.0f to 1.0;   
            public Vector2Event positionUpdate = new Vector2Event(); // triggers events(s) while passing a Vector2 parameter (the x,y position of the joystick)
            public UnityEvent down = new UnityEvent();          // triggers event(s) if button was pressed this frame
            public UnityEvent up = new UnityEvent();            // trigger event(s) if button was released this frame
            public bool isDown = false;                         // true if button is currently pressed                         
        }

        /// <summary>
        /// A group of variables for Trigger Events.
        /// </summary>
        [System.Serializable]
        public class TriggerEvents
        {
            public UnityEvent down = new UnityEvent();          // triggers event(s) if button was pressed this frame
            public UnityEvent up = new UnityEvent();            // triggers event(s) if button was released this frame
            public bool isDown = false;                         // true if button is currently pressed
            public float position;                              // range of 0.0f to 1.0f
        }

        public virtual void Start()
        {
            if(deviceType == DeviceType.leftVRController)
            {
                PuppetJumpManager.Instance.leftHandVRInputDevice = this;
            }

            if(deviceType == DeviceType.rightVRController)
            {
                PuppetJumpManager.Instance.rightHandVRInputDevice = this;
            }
        }
    }
}
