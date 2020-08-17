using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuppetJump.Utils
{
    public class PuppetStringHead : PuppetString
    {
        public PuppetHead puppetHead;           // the object being puppeted

        protected override void Start()
        {
            base.Start();

            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;   // true since the position of this string is done is FixedUpdate


        }

        /// <summary>
        /// Keeps this string in position and properly rotated.
        /// </summary>
        private void FixedUpdate()
        {
            if (PuppetJumpManager.Instance.cameraRig.cam != null)
            {
                transform.localPosition = PuppetJumpManager.Instance.cameraRig.cam.transform.localPosition;
                transform.localRotation = PuppetJumpManager.Instance.cameraRig.cam.transform.localRotation;
            }
        }
    }
}
