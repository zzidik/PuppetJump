using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuppetJump.ABSwitches;

namespace PuppetJump.Utils
{
    public class CameraRig : MonoBehaviour
    {
        public Camera cam;                      // the camera being used by this rig
        public ImageColorSwitch panelBlack;     // used for blacking out the camera

        /// <summary>
        /// Used to place the camera in a new position.
        /// </summary>
        /// <param name="newPos">The target position of the camera.</param>
        public void PositionCamera(Transform newPos)
        {
            // an empty game object used to aid in positioning
            GameObject dummy = GetDummy();

            // store the orginal parent of the camera rig
            GameObject rigOrgParent = null;
            if (transform.parent != null)
            {
                rigOrgParent = transform.parent.gameObject;
            }

            // move the camera rig so the camera is in the target position
            Debug.Log(transform.position);
            transform.position = newPos.position - cam.transform.localPosition;

            // move the dummy object to where the camera is
            dummy.transform.position = cam.transform.position;

            // rotate the dummy object so it matches the y rotation of the camera
            dummy.transform.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);

            // make the dummy object the parent of the camera rig
            transform.parent = dummy.transform;

            // rotate the dummy so it matches the rotation of the target position
            dummy.transform.rotation = Quaternion.Euler(0f, newPos.eulerAngles.y, 0f);

            // move the dummy into the target position
            dummy.transform.position = newPos.position;

            // return the camera rig to it's original parent
            if (rigOrgParent == null)
            {
                transform.parent = null;
            }
            else
            {
                transform.parent = rigOrgParent.transform;
            }

            // destroy the dummy
            DestroyImmediate(dummy);
        }
        public void PositionCamera(Vector3 rigPos, Quaternion rigRot)
        {
            transform.position = rigPos;
            transform.rotation = rigRot;
        }

        /// <summary>
        /// Searches for a dummy gameobject in the scene,
        /// if none exists it creates one.
        /// </summary>
        /// <returns>A dummy gameobject used to aid in positioning.</returns>
        public GameObject GetDummy()
        {
            GameObject dummyTrans;
            //if one exist
            if (GameObject.Find("Dummy"))
            {
                //get the Dummy transform object
                dummyTrans = GameObject.Find("Dummy");
                //if one doesnt exist
            }
            else
            {
                //create one
                dummyTrans = new GameObject();
                dummyTrans.name = "Dummy";
            }
            return dummyTrans;
        }
    }
}
