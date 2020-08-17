using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.ABSwitches
{
    /// <summary>
    /// An ABSwitch for changing an object's material.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class MaterialSwitch: ABSwitch
    {
        private Material AMaterial;             // store the start material
        public Material BMaterial;              // store a new material

        private void Awake()
        {
            AMaterial = GetComponent<Renderer>().material;
        }

        public override void Switch()
        {
            if(state == SwitchState.atA)
            {
                GetComponent<Renderer>().material = BMaterial;
                state = SwitchState.atB;
            }
            else if(state == SwitchState.atB)
            {
                GetComponent<Renderer>().material = AMaterial;
                state = SwitchState.atA;
            }
        }

        public void SwitchTo(SwitchState toState)
        {
            if (toState == SwitchState.switchingToB || toState == SwitchState.atB || toState == SwitchState.toB)
            {
                GetComponent<Renderer>().material = BMaterial;
                state = SwitchState.atB;
            }
            else if (toState == SwitchState.switchingToA || toState == SwitchState.atA || toState == SwitchState.toA)
            {
                GetComponent<Renderer>().material = AMaterial;
                state = SwitchState.atA;
            }
        }

        /// <summary>
        /// In some cases it may be necessary to reset the AMaterial after start.
        /// </summary>
        /// <param name="newMat">The new material.</param>
        public void ResetA(Material newMat)
        {
            AMaterial = newMat;
        }

        /// <summary>
        /// Sets the switch to material A.
        /// No invoke of events.
        /// </summary>
        public override void SetToA()
        {
            GetComponent<Renderer>().material = AMaterial;
            state = SwitchState.atA;
        }

        /// <summary>
        /// Sets the switch to material B.
        /// No invoke of events.
        /// </summary>
        public override void SetToB()
        {
            GetComponent<Renderer>().material = BMaterial;
            state = SwitchState.atB;
        }
    }
}
