using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace PuppetJump.Utils
{

    /// <summary>
    /// Controls a change in an object from A to B and back.
    /// </summary>
    public class ABSwitch : MonoBehaviour
    {
        [System.Serializable]
        public class SwitchEvents
        {
            public UnityEvent switchToBComplete = new UnityEvent();         // triggers event(s) when a switch to B has completed
            public UnityEvent switchToAComplete = new UnityEvent();         // triggers event(s) when a switch to A has completed
        }
        public SwitchEvents switchEvents;

        public enum SwitchState {atA, switchingToA, toA, atB, switchingToB, toB};     // the various states of a switch
        [HideInInspector]
        public SwitchState state = SwitchState.atA;                         // cuurrent state of switch
        public IEnumerator switchCoroutine;                                 // a coroutine that controls the switch
        protected bool endAtA = false;                                        // if true, during a loop, the switch loop will stop when A position is reached
        protected bool endAtB = false;                                        // if true, during a loop, the switch loop will stop when B position is reached

        /// <summary>
        /// Custom function that all ABSwitches should override for their own purposes.
        /// </summary>
        public virtual void Switch()
        {
            // unique for each ABSwitch type
        }
        /// <summary>
        /// Sets the parameters and starts the coroutine of a switch in motion
        /// towards a particular state.
        /// </summary>
        public virtual void SwitchTo(SwitchState toState)
        {
            // unique for each ABSwitch type
        }

        /// <summary>
        /// Ends a loop when a switch loop gets to the A position.
        /// </summary>
        public void EndLoopAtA()
        {
            endAtA = true;
        }

        /// <summary>
        /// Ends a loop when a switch loop gets to the B position.
        /// </summary>
        public void EndLoopAtB()
        {
            endAtB = true;
        }

        /// <summary>
        /// Sets a switch to the A position.
        /// No invoke of events.
        /// </summary>
        public virtual void SetToA()
        {
            // unique for each ABSwitch type
        }

        /// <summary>
        /// Sets a switch to the B position.
        /// No invoke of events.
        /// </summary>
        public virtual void SetToB()
        {
            // unique for each ABSwitch type
        }
    }    
}
