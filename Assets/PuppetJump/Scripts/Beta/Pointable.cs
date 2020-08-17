using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PuppetJump.Beta
{
    public class Pointable : MonoBehaviour
    {
        public bool isPointable = true;                             // can the object currently be pointed at

        [System.Serializable]
        public class PointEvents
        {
            public bool deffered = false;                           // true passses the point to a parent 
            public UnityEvent pointedAt = new UnityEvent();         // triggers event(s) if object is pointed at this frame
            public UnityEvent activate = new UnityEvent();          // triggers event(s) if object is activated while pointed at
            public UnityEvent unpointedAt = new UnityEvent();       // trigger event(s) if object is unpointed at this frame
        }
        public PointEvents pointEvents;
    }
}
