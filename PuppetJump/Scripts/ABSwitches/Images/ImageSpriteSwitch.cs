using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuppetJump.Utils;

namespace PuppetJump.ABSwitches
{
    /// <summary>
    /// An ABSwitch for changing an object's sprite of an image component.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ImageSpriteSwitch : ABSwitch
    {
        private Sprite ASprite;     // store the start sprite
        public Sprite BSprite;      // store a new sprite

        private void Awake()
        {
            ASprite = GetComponent<Image>().sprite;
        }

        public override void Switch()
        {
            if (state == SwitchState.atA)
            {
                GetComponent<Image>().sprite = BSprite;
                state = SwitchState.atB;
            }
            else if (state == SwitchState.atB)
            {
                GetComponent<Image>().sprite = ASprite;
                state = SwitchState.atA;
            }
        }

        public void SwitchTo(SwitchState toState)
        {
            if (toState == SwitchState.switchingToB || toState == SwitchState.atB || toState == SwitchState.toB)
            {
                GetComponent<Image>().sprite = BSprite;
                state = SwitchState.atB;
            }
            else if (toState == SwitchState.switchingToA || toState == SwitchState.atA || toState == SwitchState.toA)
            {
                GetComponent<Image>().sprite = ASprite;
                state = SwitchState.atA;
            }
        }

        /// <summary>
        /// In some cases it may be necessary to reset the ASprite after start.
        /// </summary>
        /// <param name="newSprite">The new sprite.</param>
        public void ResetA(Sprite newSprite)
        {
            ASprite = newSprite;
        }

        /// <summary>
        /// Sets the switch to sprite A.
        /// No invoke of events.
        /// </summary>
        public override void SetToA()
        {
            GetComponent<Image>().sprite = ASprite;
            state = SwitchState.atA;
        }

        /// <summary>
        /// Sets the switch to sprite B.
        /// No invoke of events.
        /// </summary>
        public override void SetToB()
        {
            GetComponent<Image>().sprite = BSprite;
            state = SwitchState.atB;
        }
    }
}
