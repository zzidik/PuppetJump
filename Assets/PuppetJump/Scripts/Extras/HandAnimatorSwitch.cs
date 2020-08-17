using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump;

namespace PuppetJump.Extras
{
    public class HandAnimatorSwitch : MonoBehaviour
    {
        public UnityEditor.Animations.AnimatorController oculusController;
        public UnityEditor.Animations.AnimatorController viveController;

        private void Start()
        {
            switch (PuppetJumpManager.Instance.deviceType)
            {
                case PuppetJumpManager.DeviceTypes.OVR:
                    GetComponent<Animator>().runtimeAnimatorController = oculusController;
                    break;
                case PuppetJumpManager.DeviceTypes.SteamVR:
                    GetComponent<Animator>().runtimeAnimatorController = viveController;
                    break;
            }
        }
    }
}
