using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using PuppetJump.Objs;
using PuppetJump.Utils;
using PuppetJump.ABSwitches;

namespace PuppetJump.Utils
{
    public class ControllerDisplayObject : MonoBehaviour
    {
        [System.Serializable]
        public class InstructionBox
        {
            public GameObject box;
            public Text title;
            public Text instruction;
        }

        [System.Serializable]
        public class InstructionBoxes
        {
            public InstructionBox trigger;
            public InstructionBox grip;
            public InstructionBox buttonOne;
            public InstructionBox buttonTwo;
            public InstructionBox thumbStick;
            public InstructionBox buttonToucher;
        }

        [System.Serializable]
        public class Glows
        {
            public List<MaterialEmmissionColorSwitch> buttonTouchers = new List<MaterialEmmissionColorSwitch>();
            public MaterialEmmissionColorSwitch trigger;
            public MaterialEmmissionColorSwitch buttonOne;
            public MaterialEmmissionColorSwitch buttonTwo;
            public MaterialEmmissionColorSwitch grip;
            public MaterialEmmissionColorSwitch thumbStick;
            public MaterialEmmissionColorSwitch menu;
        }

        public enum Hands { left, right };
        public Hands hand;

        public enum Types { OVR, SteamVR };
        [ReadOnly]
        public Types type;

        [System.Serializable]
        public class Display
        {
            public GameObject controller;
            public InstructionBoxes instructionBoxes = new InstructionBoxes();  // gives access to instruction boxes for help with controls in game
            public Glows glows = new Glows();                                   // gives access to glows on buttons of the controllers
        }
        public Display displayOVR;
        public Display displaySteamVR;

        private void Start()
        {
            switch (PuppetJumpManager.Instance.deviceType)
            {
                case PuppetJumpManager.DeviceTypes.OVR:
                    type = Types.OVR;
                    break;
                case PuppetJumpManager.DeviceTypes.SteamVR:
                    type = Types.SteamVR;
                    break;
            }

            // set the display type
            SetDisplayType(type);
        }

        /// <summary>
        /// Sets the display of a controller.
        /// </summary>
        /// <param name="type">Which type of display to use.</param>
        public void SetDisplayType(Types type)
        {
            switch (type)
            {
                case Types.OVR:
                    displayOVR.controller.SetActive(true);
                    displaySteamVR.controller.SetActive(false);
                    break;
                case Types.SteamVR:
                    displayOVR.controller.SetActive(false);
                    displaySteamVR.controller.SetActive(true);
                    break;
            }

            type = type;
        }

        /// <summary>
        /// Hides and shows all controller meshes.
        /// </summary>
        /// <param name="hide">True if all meshes are hidden.</param>
        public void HideDisplay(bool hide)
        {
            if (hide)
            {
                displayOVR.controller.SetActive(false);
                displaySteamVR.controller.SetActive(false);
            }
            else
            {
                SetDisplayType(type);
            }
        }

        /// <summary>
        /// Sets all instruction boxes to inactive.
        /// </summary>
        public void HideAllInstructionBoxes()
        {
            switch (type)
            {
                case Types.OVR:
                    displayOVR.instructionBoxes.trigger.box.SetActive(false);
                    displayOVR.instructionBoxes.grip.box.SetActive(false);
                    displayOVR.instructionBoxes.buttonOne.box.SetActive(false);
                    displayOVR.instructionBoxes.buttonTwo.box.SetActive(false);
                    displayOVR.instructionBoxes.thumbStick.box.SetActive(false);
                    displayOVR.instructionBoxes.buttonToucher.box.SetActive(false);
                    break;
                case Types.SteamVR:
                    displaySteamVR.instructionBoxes.trigger.box.SetActive(false);
                    displaySteamVR.instructionBoxes.grip.box.SetActive(false);
                    displaySteamVR.instructionBoxes.buttonOne.box.SetActive(false);
                    displaySteamVR.instructionBoxes.buttonTwo.box.SetActive(false);
                    displaySteamVR.instructionBoxes.thumbStick.box.SetActive(false);
                    displaySteamVR.instructionBoxes.buttonToucher.box.SetActive(false);
                    break;
            }
        }
    }
}