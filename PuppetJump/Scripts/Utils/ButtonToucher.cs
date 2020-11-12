using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Objs;

namespace PuppetJump.Utils
{
    public class ButtonToucher : MonoBehaviour
    {
        [ReadOnly]
        public PuppetJump.Objs.Button touchedButton;    // an object being touched, there can be only one at a time

        public virtual void OnTriggerEnter(Collider other)
        {
            AssignTouch(other);
        }

        public virtual void OnTriggerExit(Collider other)
        {
            // only check this is there is a touchedButton defined
            if (touchedButton != null)
            {
                // end it
                EndTouch();
            }
        }


        /// <summary>
        /// Checks if a collision results in the defining of a touchedObject.
        /// </summary>
        /// <param name="other"></param>
        public void AssignTouch(Collider other)
        {
            // if the object collided with is a Button
            if (other.GetComponent<PuppetJump.Objs.Button>())
            {
                // is it currently touchable
                if (other.GetComponent<PuppetJump.Objs.Button>().isTouchable)
                {
                    // if there is already a touchedObject and it's different that the new collision
                    if (touchedButton != null && touchedButton != other.GetComponent<PuppetJump.Objs.Button>())
                    {
                        // tell the previous touchedObject to not be
                        EndTouch();
                    }

                    // check to see if the puppetHand is pointing at the object
                    if (IsPointingAtTouchedObject(other.GetComponent<PuppetJump.Objs.Button>().gameObject))
                    {
                        // new collision becomes the touched object
                        touchedButton = other.GetComponent<PuppetJump.Objs.Button>();
                    }
                }
            }

            // if we have a touchedButton
            if (touchedButton != null)
            {
                // pass the toucher doing the touching to the object
                touchedButton.toucherTouching = this;
                // indicate the object is being touched 
                touchedButton.Touch();
            }
        }

        /// <summary>
        /// Ends the touch of a touchedButton.
        /// </summary>
        public void EndTouch()
        {
            if (touchedButton != null)
            {
                // tell the object no toucher is touching it
                touchedButton.toucherTouching = null;
                // indicate the object is not being touched
                touchedButton.Untouch();
            }

            // clear the touchObject
            touchedButton = null;
        }

        /// <summary>
        /// Checks if this toucher is pointing at the object it is touching.
        /// </summary>
        /// <returns>True if it is.</returns>
        public bool IsPointingAtTouchedObject(GameObject go)
        {
            bool isPointingAt = false;

            // create a ray that points forwards from this hand
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // if this hand is pointing at the object it is touching
                if (hit.collider == go.GetComponent<Collider>())
                {
                    isPointingAt = true;
                }
            }
            

            return isPointingAt;
        }
    }
}
