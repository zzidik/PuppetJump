using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump;

namespace PuppetJump.Utils
{
    [RequireComponent(typeof(CharacterController))]

    public class PuppetRig : MonoBehaviour
    {
        protected Vector3 moveDirection = Vector3.zero;
        protected Vector3 forwardDirection = Vector3.zero;
        protected Vector3 strafeDirection = Vector3.zero;
        [HideInInspector]
        public CharacterController characterController;

        protected float rotDir;
        public float gravity = 20.0f;
        [System.Serializable]
        public class Speeds
        {
            public float move = 3.0f;
            public float strafe = 3.0f;
            public float rotation = 120f;
            public float jump = 8.0f;
        }
        public Speeds speeds;
        protected bool isJumping = false;

        public enum ViewTypes { firstPerson, thirdPerson };
        [System.Serializable]
        public class Views
        {
            public Transform firstPersonView;           // the position of the first person camera
            public Transform thridPersonView;           // the position of the third person camera
            public ViewTypes view;                      // the current view
        }
        public Views views;
        protected bool initialized = false;

        protected virtual void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        public virtual void SetView(ViewTypes view)
        {
            switch (view)
            {
                case ViewTypes.firstPerson:
                    // switch to first person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(views.firstPersonView);
                    // make the puppet the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = transform;
                    views.view = ViewTypes.firstPerson;
                    break;
                case ViewTypes.thirdPerson:
                    // make the world the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = null;
                    // switch to third person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(views.thridPersonView);
                    views.view = ViewTypes.thirdPerson;
                    break;
            }
        }
        public virtual void SetView(ViewTypes view, Vector3 rigPos, Quaternion rigRot)
        {
            switch (view)
            {
                case ViewTypes.firstPerson:
                    // switch to first person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(rigPos, rigRot);
                    // make the puppet the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = transform;
                    views.view = ViewTypes.firstPerson;
                    break;
                case ViewTypes.thirdPerson:
                    // make the world the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = null;
                    // switch to third person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(rigPos, rigRot);
                    views.view = ViewTypes.thirdPerson;
                    break;
            }
        }

        public virtual void ChangeView()
        {
            switch (views.view)
            {
                // if currently in first person view
                case ViewTypes.firstPerson:
                    // make the world the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = null;
                    // switch to third person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(views.thridPersonView);
                    views.view = ViewTypes.thirdPerson;
                    break;
                case ViewTypes.thirdPerson:
                    // switch to first person view
                    PuppetJumpManager.Instance.cameraRig.PositionCamera(views.firstPersonView);
                    // make the puppet the parent of the camera rig
                    PuppetJumpManager.Instance.cameraRig.transform.parent = transform;
                    views.view = ViewTypes.firstPerson;
                    break;
            }
        }

        public virtual void Jump()
        {
            isJumping = true;
        }
    }
}
