using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuppetJump.Extras
{
    public class HandStateController : MonoBehaviour
    {
        public Animator animator;

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
