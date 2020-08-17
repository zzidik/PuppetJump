using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump;

namespace PuppetJump.Utils
{

    public class PuppetRigDesktop : PuppetRig
    {
        [System.Serializable]
        public class MouseBasedRotation
        {
            public Transform subject;
            public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 };
            public RotationAxes axes = RotationAxes.MouseXAndY;

            public float sensitivityX = 15F;
            public float sensitivityY = 15F;

            public float minimumX = -360F;
            public float maximumX = 360F;

            public float minimumY = -60F;
            public float maximumY = 60F;

            float rotationX = 0F;
            float rotationY = 0F;

            public Quaternion originalRotation;
            private Quaternion targetRotation;
            private float rotSpeed = 0.1f;

            public void RotateWithMouse()
            {
                if (axes == RotationAxes.MouseXAndY)
                {
                    // Read the mouse input axis
                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                    rotationX = ClampAngle(rotationX, minimumX, maximumX);
                    rotationY = ClampAngle(rotationY, minimumY, maximumY);

                    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                    Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                    //subject.localRotation = originalRotation * xQuaternion * yQuaternion;
                    targetRotation = originalRotation * xQuaternion * yQuaternion;
                }
                else if (axes == RotationAxes.MouseX)
                {
                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                    rotationX = ClampAngle(rotationX, minimumX, maximumX);

                    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                    //subject.localRotation = originalRotation * xQuaternion;
                    targetRotation = originalRotation * xQuaternion;
                }
                else
                {
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = ClampAngle(rotationY, minimumY, maximumY);

                    Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                    //subject.localRotation = originalRotation * yQuaternion;
                    targetRotation = originalRotation * yQuaternion;
                }

                subject.localRotation = Quaternion.Lerp(subject.localRotation, targetRotation, Time.time * rotSpeed);
            }
        }
        public MouseBasedRotation mouseRotCam;
        public MouseBasedRotation mouseRotPuppet;
        public Transform thirdPersonLookAtTarget;


        public void InitializePuppet()
        {
            mouseRotCam.subject = PuppetJumpManager.Instance.cameraRig.cam.transform;
            mouseRotCam.originalRotation = PuppetJumpManager.Instance.cameraRig.cam.transform.localRotation;
            mouseRotCam.axes = MouseBasedRotation.RotationAxes.MouseY;

            mouseRotPuppet.subject = transform;
            mouseRotPuppet.originalRotation = transform.localRotation;
            mouseRotPuppet.axes = MouseBasedRotation.RotationAxes.MouseX;

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
            if (!initialized && PuppetJumpManager.Instance.deviceType == PuppetJumpManager.DeviceTypes.Desktop)
            {
                InitializePuppet();
            }

            // MOVEMENT
            /////////////////////////////////////////////////////////////////////////
            if (characterController.isGrounded)
            {
                forwardDirection = new Vector3(transform.forward.x, 0, transform.forward.z);
                strafeDirection = new Vector3(transform.right.x, 0, transform.right.z);


                if (Input.GetKey("left") || Input.GetKey("a"))
                {
                    rotDir = -1f;
                    strafeDirection *= -1f * speeds.strafe;
                }
                else if (Input.GetKey("right") || Input.GetKey("d"))
                {
                    rotDir = 1f;
                    strafeDirection *= 1f * speeds.strafe;
                }
                else
                {
                    rotDir = 0f;
                    strafeDirection *= 0f;
                }

                if (Input.GetKey("up") || Input.GetKey("w"))
                {
                    forwardDirection *= 1f * speeds.move;
                }
                else if (Input.GetKey("down") || Input.GetKey("s"))
                {
                    forwardDirection *= -1f * speeds.move;
                }
                else
                {
                    forwardDirection *= 0f;
                }

                if (views.view == ViewTypes.firstPerson)
                {
                    moveDirection = forwardDirection + strafeDirection;
                }
                else
                {
                    moveDirection = forwardDirection;
                }

                if (Input.GetButton("Jump"))
                {
                    isJumping = true;
                }

                if (isJumping)
                {
                    moveDirection.y = speeds.jump;
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
                    mouseRotCam.RotateWithMouse();
                    mouseRotPuppet.RotateWithMouse();
                    break;
                case ViewTypes.thirdPerson:
                    // rotate the puppet
                    transform.RotateAround(transform.position, Vector3.up, rotDir * speeds.rotation * Time.deltaTime);
                    PuppetJumpManager.Instance.cameraRig.cam.transform.LookAt(thirdPersonLookAtTarget);
                    break;
            }
            /////////////////////////////////////////////////////////////////////////
            // END ROTATION

            if (Input.GetKeyDown("v"))
            {
                ChangeView();
            }
        }

        private void FixedUpdate()
        {
            // the first person view follows the local rotation of the camera
            views.firstPersonView.localRotation = PuppetJumpManager.Instance.cameraRig.cam.transform.localRotation;
        }

        public override void ChangeView()
        {
            base.ChangeView();

            switch (views.view)
            {
                case ViewTypes.firstPerson:
                    mouseRotCam.axes = MouseBasedRotation.RotationAxes.MouseY;
                    break;
                case ViewTypes.thirdPerson:
                    mouseRotCam.axes = MouseBasedRotation.RotationAxes.MouseXAndY;
                    break;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
