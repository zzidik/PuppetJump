using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Objs;

namespace PuppetJump.Utils
{
    public class PuppetStringHand : PuppetString
    {
        public enum ControlledBy { LeftHand, RightHand };
        public ControlledBy controlledBy;
        [HideInInspector]
        public GameObject controller;                   // the target to pull the controlled end of the string towards

        [System.Serializable]
        public class ForceToPosition
        {
            [ReadOnly]
            public Transform target;
            public float frequency = 1.0f;
            public float damping = 1.0f;
            public bool massEffectsDamping = true;
        }
        public ForceToPosition forceToPosition;         // settings to add force to the string towards to controller

        [System.Serializable]
        public class TorqueToRotation
        {
            [ReadOnly]
            public Transform target;
            public float frequency = 1.0f;
            public float damping = 1.0f;
            public bool massEffectsDamping = true;
        }
        public TorqueToRotation torqueToRotation;       // settings to add torque to the string towards to controller
        public PuppetHand puppetHand;                   // the object being puppeted

        [ReadOnly]
        public FixedJoint grabJointFixed;               // a fixed joint used to attach fixed joint grabbable objects

        protected override void Start()
        {
            base.Start();

            controller = new GameObject();
            controller.transform.parent = PuppetJumpManager.Instance.puppetRig.transform;
            forceToPosition.target = controller.transform;
            torqueToRotation.target = controller.transform;
            switch (controlledBy)
            {
                case ControlledBy.LeftHand:
                    controller.name = "PuppetStringController [LeftHand]";
                    puppetHand.forceToPosition.target = controller.transform;
                    puppetHand.torqueToRotation.target = controller.transform;
                    break;
                case ControlledBy.RightHand:
                    controller.name = "PuppetStringController [RightHand]";
                    puppetHand.forceToPosition.target = controller.transform;
                    puppetHand.torqueToRotation.target = controller.transform;
                    break;
            }
        }

        /// <summary>
        /// Keeps the controllers in their proper positions and rotations.
        /// Adds force and torque to the string towards the controller.
        /// </summary>
        private void FixedUpdate()
        {
            switch (controlledBy)
            {
                case ControlledBy.LeftHand:
                    if (PuppetJumpManager.Instance.leftHandVRInputDevice != null)
                    {
                        controller.transform.localPosition = PuppetJumpManager.Instance.leftHandVRInputDevice.transform.localPosition;
                        controller.transform.localRotation = PuppetJumpManager.Instance.leftHandVRInputDevice.transform.localRotation;
                    }
                    break;
                case ControlledBy.RightHand:
                    if (PuppetJumpManager.Instance.rightHandVRInputDevice != null)
                    {
                        controller.transform.localPosition = PuppetJumpManager.Instance.rightHandVRInputDevice.transform.localPosition;
                        controller.transform.localRotation = PuppetJumpManager.Instance.rightHandVRInputDevice.transform.localRotation;
                    }
                    break;
            }

            if (rigidbody != null)
            {
                AddForceToPosition();
                AddTorqueToRotation();
            }
        }

        void AddForceToPosition()
        {
            Vector3 Pdes = forceToPosition.target.position;
            Vector3 Vdes = Vector3.zero;

            float massDamp = forceToPosition.damping;
            if (forceToPosition.massEffectsDamping)
            {
                massDamp *= rigidbody.mass;
            }

            float kp = (6f * forceToPosition.frequency) * (6f * forceToPosition.frequency) * 0.25f;
            float kd = 4.5f * forceToPosition.frequency * massDamp;

            float dt = Time.fixedDeltaTime;
            float g = 1 / (1 + kd * dt + kp * dt * dt);
            float ksg = kp * g;
            float kdg = (kd + kp * dt) * g;
            Vector3 Pt0 = transform.position;
            Vector3 Vt0 = rigidbody.velocity;
            Vector3 F = (Pdes - Pt0) * ksg + (Vdes - Vt0) * kdg;
            rigidbody.AddForce(F);
        }

        void AddTorqueToRotation()
        {
            Quaternion desiredRotation = torqueToRotation.target.rotation;

            float massDamp = torqueToRotation.damping;
            if (torqueToRotation.massEffectsDamping)
            {
                massDamp *= rigidbody.mass;
            }

            float kp = (6f * torqueToRotation.frequency) * (6f * torqueToRotation.frequency) * 0.25f;
            float kd = 4.5f * torqueToRotation.frequency * torqueToRotation.damping;
            float dt = Time.fixedDeltaTime;
            float g = 1 / (1 + kd * dt + kp * dt * dt);
            float ksg = kp * g;
            float kdg = (kd + kp * dt) * g;
            Vector3 x;
            float xMag;
            Quaternion q = desiredRotation * Quaternion.Inverse(transform.rotation);
            q.ToAngleAxis(out xMag, out x);
            x.Normalize();
            x *= Mathf.Deg2Rad;
            Vector3 pidv = kp * x * xMag - kd * rigidbody.angularVelocity;
            Quaternion rotInertia2World = rigidbody.inertiaTensorRotation * transform.rotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(rigidbody.inertiaTensor);
            pidv = rotInertia2World * pidv;
            rigidbody.AddTorque(pidv);
        }

        /// <summary>
        /// If a PuppetHand is grabbing an object with a FixedJoint,
        /// and the joint breaks because too much force is placed on it,
        /// this releases the grabbed object.
        /// </summary>
        void OnJointBreak()
        {
            if (puppetHand.grabbedObject != null)
            {
                if (puppetHand.grabbedObject.GetComponent<Grabbable>().grabStyle == Grabbable.GrabStyles.fixedJoint)
                {
                    puppetHand.grabbedObject.GetComponent<Grabbable>().Release(puppetHand);
                }
            }
        }
    }
}
