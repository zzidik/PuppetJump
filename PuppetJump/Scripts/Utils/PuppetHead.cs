using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuppetJump.Utils
{
    public class PuppetHead : MonoBehaviour
    {
        [HideInInspector]
        public Rigidbody rigidbody;
        public bool blackOutOnCollision = true;

        private void Start()
        {
            // lock rotation on the head rigidbody
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (blackOutOnCollision)
            {
                // black out the camera
                if (PuppetJumpManager.Instance.puppetRig.views.view == PuppetRig.ViewTypes.firstPerson)
                {
                    PuppetJumpManager.Instance.cameraRig.panelBlack.SwitchTo(ABSwitch.SwitchState.toB);
                }
            }
        }

        protected virtual void OnCollisionStay(Collision collision)
        {
            if (blackOutOnCollision)
            {
                if (PuppetJumpManager.Instance.puppetRig.views.view == PuppetRig.ViewTypes.firstPerson)
                {
                    // black out the camera
                    if (PuppetJumpManager.Instance.cameraRig.panelBlack.state != ABSwitch.SwitchState.atB)
                    {
                        PuppetJumpManager.Instance.cameraRig.panelBlack.SwitchTo(ABSwitch.SwitchState.toB);
                    }
                }
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (blackOutOnCollision)
            {
                if (PuppetJumpManager.Instance.puppetRig.views.view == PuppetRig.ViewTypes.firstPerson)
                {
                    // unblack out the camera
                    PuppetJumpManager.Instance.cameraRig.panelBlack.SwitchTo(ABSwitch.SwitchState.toA);
                }
            }
        }

        private void Update()
        {
            if (blackOutOnCollision)
            {
                if (PuppetJumpManager.Instance.puppetRig.views.view == PuppetRig.ViewTypes.thirdPerson && PuppetJumpManager.Instance.cameraRig.panelBlack.state != ABSwitch.SwitchState.atA)
                {
                    // sets the panelBlack clear
                    PuppetJumpManager.Instance.cameraRig.panelBlack.SetToA();
                }
            }
        }
    }
}
