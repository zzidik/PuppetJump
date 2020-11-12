using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PuppetJump;
using PuppetJump.Utils;

namespace PuppetJump.Objs
{
    public class Button : MonoBehaviour
    {
        public bool isTouchable = true;                         // can the object currently be touched
        [HideInInspector]
        public bool isTouched = false;                          // is the object currently being touched
        [HideInInspector]
        public bool wasTouched = false;                         // a check if the object was previously declared touched or not, prevents constant touching events
        [HideInInspector]
        public ButtonToucher toucherTouching;                   // used to a reference to the toucher that is touching the object        
        [System.Serializable]
        public class TouchEvents
        {
            public UnityEvent touch = new UnityEvent();         // triggers event(s) if object is touched this frame
            public UnityEvent untouch = new UnityEvent();       // trigger event(s) if object is untouched this frame
        }
        public TouchEvents touchEvents;
        public bool isRadioButton = false;          // does the button get locked down when let go, and unlock when touched again
        public bool isRadioButtonDown = false;      // keep track of the state of the radio button

        public virtual void Touch()
        {
            isTouched = true;

            // if a toucher has touched this object
            // and it has not already been labled as touched
            if (!wasTouched)
            {
                if (isRadioButton)
                {
                    // switch the radio button to the oppositie state
                    if (isRadioButtonDown)
                    {
                        isRadioButtonDown = false;
                    }
                    else
                    {
                        isRadioButtonDown = true;
                    }
                }

                wasTouched = true;
                touchEvents.touch.Invoke();
            }
        }

        public virtual void Untouch()
        {
            isTouched = false;

            // if a toucher has stopped touching this object
            // and it is still labled as touched
            if (wasTouched)
            {
                wasTouched = false;
                touchEvents.untouch.Invoke();
            }
        }

        private void OnDisable()
        {
            if (toucherTouching != null)
            {
                toucherTouching.touchedButton = null;
                toucherTouching = null;
                isTouched = false;
                wasTouched = false;
            }
        }
    }
}
