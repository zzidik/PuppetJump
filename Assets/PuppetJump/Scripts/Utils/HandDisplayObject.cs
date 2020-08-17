using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuppetJump.Utils
{
    public class HandDisplayObject : MonoBehaviour
    {
        public UnityEditor.Animations.AnimatorController oculusController;
        public UnityEditor.Animations.AnimatorController viveController;
        public Animator animator;

        private void Update()
        {
            if (PuppetJumpManager.Instance.vrReady && animator.runtimeAnimatorController == null)
            {
                switch (PuppetJumpManager.Instance.deviceType)
                {
                    case PuppetJumpManager.DeviceTypes.OVR:
                        animator.runtimeAnimatorController = oculusController;
                        break;
                    case PuppetJumpManager.DeviceTypes.SteamVR:
                        animator.runtimeAnimatorController = viveController;
                        break;
                }
            }
        }

        public void TriggerButton(bool down)
        {
            animator.SetBool("trigger", down);
        }

        public void GripButton(bool down)
        {
            animator.SetBool("grip", down);
        }

        public void ButtonOne(bool down)
        {
            animator.SetBool("buttonOne", down);
        }

        public void ButtonTwo(bool down)
        {
            animator.SetBool("buttonTwo", down);
        }

        public void Joystick(bool down)
        {
            animator.SetBool("joystick", down);
        }
    }
}
