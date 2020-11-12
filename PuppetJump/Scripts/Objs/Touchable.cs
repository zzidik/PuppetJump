using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PuppetJump.Utils;

namespace PuppetJump.Objs
{
    [RequireComponent(typeof(Collider))]
    public class Touchable : MonoBehaviour
    {
        public bool isTouchable = true;                         // can the object currently be touched
        [ReadOnly]
        public bool isTouched = false;                          // is the object currently being touched
        [ReadOnly]
        public bool wasTouched = false;                         // a check if the object was previously declared touched or not, prevents constant touching events
        [ReadOnly]
        public PuppetHand puppetHandTouching;                   // used to a reference to the PuppetHand that is touching the object        
        [System.Serializable]
        public class TouchEvents
        {
            public bool deffered = false;                       // true passses the touch to a parent 
            public UnityEvent touch = new UnityEvent();         // triggers event(s) if object is touched this frame
            public UnityEvent untouch = new UnityEvent();       // trigger event(s) if object is untouched this frame
        }
        public TouchEvents touchEvents;
       
        public virtual void Touch()
        {
            isTouched = true;

            // if a toucher has touched this object
            // and it has not already been labled as touched
            if (!wasTouched)
            {
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
            if(puppetHandTouching != null)
            {
                puppetHandTouching.touchedObject = null;
                puppetHandTouching = null;
                isTouched = false;
                wasTouched = false;
            }
        }
    }
}