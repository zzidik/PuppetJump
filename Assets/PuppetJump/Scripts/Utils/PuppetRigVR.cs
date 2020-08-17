using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump;

namespace PuppetJump.Utils
{
    public class PuppetRigVR : PuppetRig
    {
        [System.Serializable]
        public class PuppetStrings
        {
            public PuppetStringHead head;
            public PuppetStringHand leftHand;
            public PuppetStringHand rightHand;
        }
        public PuppetStrings puppetStrings;

        private Vector2 moveInput;
        public Vector2 MoveInput
        {
            get
            {
                return moveInput;
            }
            set
            {
                moveInput = value;
            }
        }

        public void InitializePuppet()
        {
            // ignore collisions bewtween the PuppetHead and the PuppetHands and the CharacterCollider
            foreach (var c in puppetStrings.head.puppetHead.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(c, GetComponent<CharacterController>());
            }

            foreach (var c in puppetStrings.leftHand.puppetHand.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(c, GetComponent<CharacterController>());
            }
                
            foreach (var c in puppetStrings.rightHand.puppetHand.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(c, GetComponent<CharacterController>());
            }            

            switch (views.view)
            {
                case ViewTypes.firstPerson:
                    // move the camera rig so the camera is in the first person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(views.firstPersonView);
                    // make the puppet the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = transform;
                    break;
                case ViewTypes.thirdPerson:
                    // make the world the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = null;
                    // move the camera rig so the camera is in the thrid person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(views.thridPersonView);
                    break;
            }

            initialized = true;
        }

        private void Update()
        {
            if (!initialized && PuppetJumpManager.Instance.vrReady)
            {
                InitializePuppet();
            }

            // MOVEMENT
            /////////////////////////////////////////////////////////////////////////
            if (characterController.isGrounded)
            {
                forwardDirection = new Vector3(puppetStrings.head.transform.forward.x, 0, puppetStrings.head.transform.forward.z);
                strafeDirection = new Vector3(puppetStrings.head.transform.right.x, 0, puppetStrings.head.transform.right.z);
                forwardDirection *= moveInput.y * speeds.move;
                switch (views.view)
                {
                    case ViewTypes.firstPerson:
                        strafeDirection *= moveInput.x * speeds.strafe;
                        break;
                    case ViewTypes.thirdPerson:
                        strafeDirection *= 0f;
                        break;
                }
                moveDirection = forwardDirection + strafeDirection;

                if (isJumping)
                {
                    moveDirection.y = speeds.jump;
                }

                // set character controller height
                characterController.height = puppetStrings.head.transform.localPosition.y;
                characterController.center = new Vector3(characterController.center.x, characterController.height * 0.5f, characterController.center.z);

                // set charcater controller position, only when moving with code (position will not change when user moves physically)
                if (moveDirection != Vector3.zero)
                {
                    characterController.center = new Vector3(puppetStrings.head.transform.localPosition.x, characterController.center.y, puppetStrings.head.transform.localPosition.z);
                }
            }
            else
            {
                if (isJumping)
                {
                    isJumping = false;
                }
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the controller
            characterController.Move(moveDirection * Time.deltaTime);
            /////////////////////////////////////////////////////////////////////////
            // END MOVEMENT

            // ROTATION
            /////////////////////////////////////////////////////////////////////////
            switch (views.view)
            {
                case ViewTypes.firstPerson:
                    // rotation is controller by HMD
                    break;
                case ViewTypes.thirdPerson:
                    // rotate the puppetRig
                    transform.RotateAround(puppetStrings.head.transform.position, Vector3.up, moveInput.x * speeds.rotation * Time.deltaTime);
                    break;
            }
            /////////////////////////////////////////////////////////////////////////
            // END ROTATION
        }

        private void FixedUpdate()
        {
            // ???
            //views.firstPersonView.localRotation = Quaternion.identity;
        }
    }
}
