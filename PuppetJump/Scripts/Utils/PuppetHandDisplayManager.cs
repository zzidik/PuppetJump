using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuppetJump.Utils
{
    [RequireComponent(typeof(PuppetHand))]
    public class PuppetHandDisplayManager : MonoBehaviour
    {
        public ControllerDisplayObject controllerDisplayObject;
        public HandDisplayObject handDisplayObject;
        public enum Display { controller, hand, both };
        public Display display;

        private void Start()
        {
            SetDisplayType(display);
        }

        /// <summary>
        /// Sets the display object.
        /// </summary>
        /// <param name="type">Which type of display to use.</param>
        public void SetDisplayType(Display displayThis)
        {
            switch (displayThis)
            {
                case Display.controller:
                    controllerDisplayObject.gameObject.SetActive(true);
                    handDisplayObject.gameObject.SetActive(false);
                    break;
                case Display.hand:
                    controllerDisplayObject.gameObject.SetActive(false);
                    handDisplayObject.gameObject.SetActive(true);
                    break;
                case Display.both:
                    controllerDisplayObject.gameObject.SetActive(true);
                    handDisplayObject.gameObject.SetActive(true);
                    break;
            }

            display = displayThis;
        }

        /// <summary>
        /// Cycles through the display objects in order.
        /// </summary>
        public void CycleDisplayType()
        {
            switch (display)
            {
                case Display.controller:
                    controllerDisplayObject.gameObject.SetActive(false);
                    handDisplayObject.gameObject.SetActive(true);
                    display = Display.hand;
                    break;
                case Display.hand:
                    controllerDisplayObject.gameObject.SetActive(true);
                    handDisplayObject.gameObject.SetActive(true);
                    display = Display.both;
                    break;
                case Display.both:
                    controllerDisplayObject.gameObject.SetActive(true);
                    handDisplayObject.gameObject.SetActive(false);
                    display = Display.controller;
                    break;
            }
        }

        /// <summary>
        /// Hides and shows all display objects.
        /// </summary>
        /// <param name="hide">True if all display objects are hidden.</param>
        public void HideDisplay(bool hide)
        {
            if (hide)
            {
                controllerDisplayObject.gameObject.SetActive(false);
                handDisplayObject.gameObject.SetActive(false);
            }
            else
            {
                SetDisplayType(display);
            }
        }
    }
}
