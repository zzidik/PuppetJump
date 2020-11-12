using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Objs;
using PuppetJump.Beta;

namespace PuppetJump.Utils
{
    public class PuppetHand : MonoBehaviour
    {
        public enum Types { rightHandVR, leftHandVR, other };
        public Types type;
        public PuppetStringHand puppetString;   // the PuppetString this PuppetHand is attached to
        public GameObject displayObject;        // an object that represents the hand
        public PuppetHandDisplayManager puppetHandDisplayManager;
        [HideInInspector]
        public Rigidbody rigidbody;
        [System.Serializable]
        public class ForceToPosition
        {
            [ReadOnly]
            public Transform target;
            public float frequency = 1.0f;
            public float damping = 1.0f;
            public bool massEffectsDamping = true;
        }
        public ForceToPosition forceToPosition;

        [System.Serializable]
        public class TorqueToRotation
        {
            [ReadOnly]
            public Transform target;
            public float frequency = 1.0f;
            public float damping = 1.0f;
            public bool massEffectsDamping = true;
        }
        public TorqueToRotation torqueToRotation;
        [ReadOnly]
        public GameObject touchedObject;        // an object being touched
        [ReadOnly]
        public GameObject grabbedObject;        // an object being grabbed
        public bool grabAllConnected = false;   // true allows a grab to grab an entire stack of connected connectables
        [HideInInspector]
        public float originalMass;
        

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            originalMass = rigidbody.mass;
        }

        private void OnCollisionEnter(Collision collision)
        {
            int contactNum = collision.contactCount;
            for (int c = 0; c < contactNum; c++)
            {
                if (collision.GetContact(c).otherCollider.gameObject.GetComponent<Touchable>() && grabbedObject == null)
                {
                    AssignTouch(collision.GetContact(c).otherCollider.gameObject.GetComponent<Collider>());
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (touchedObject == null)
            {
                int contactNum = collision.contactCount;
                for (int c = 0; c < contactNum; c++)
                {
                    if (collision.GetContact(c).otherCollider.gameObject.GetComponent<Touchable>() && grabbedObject == null)
                    {
                        AssignTouch(collision.GetContact(c).otherCollider.gameObject.GetComponent<Collider>());
                    }
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (touchedObject != null)
            {
                EndTouch();
            }
        }

        private void FixedUpdate()
        {
            if (rigidbody != null)
            {
                AddForceToPosition();
                AddTorqueToRotation();
            }
        }

        /// <summary>
        /// Checks if a collision results in the defining of a touchedObject.
        /// </summary>
        /// <param name="other"></param>
        public void AssignTouch(Collider other)
        {
            // if the object collided with is a Touchable object 
            if (other.gameObject.GetComponent<Touchable>())
            {
                // is it currently touchable
                if (other.gameObject.GetComponent<Touchable>().isTouchable)
                {
                    // is the touch of this object deferred to a parent
                    if (other.gameObject.GetComponent<Touchable>().touchEvents.deffered)
                    {
                        GameObject touchableParent = PuppetJumpManager.Instance.GetTouchableAncestor(other.gameObject);
                        if (touchableParent != null)
                        {
                            // if there is already a touchedObject and it's different that the new collision
                            if (touchedObject != null && touchedObject != touchableParent)
                            {
                                // tell the previous touchedObject to not be
                                EndTouch();
                            }

                            // new collision becomes the touched object
                            touchedObject = touchableParent;
                        }
                    }
                    else
                    {
                        // if there is already a touchedObject and it's different that the new collision
                        if (touchedObject != null && touchedObject != other.gameObject)
                        {
                            // tell the previous touchedObject to not be
                            EndTouch();
                        }

                        // new collision becomes the touched object
                        touchedObject = other.gameObject;
                    }
                }
            }

            // if we have a touchedObject
            if (touchedObject != null)
            {
                // pass the PuppetHand doing the touching to the object
                touchedObject.GetComponent<Touchable>().puppetHandTouching = this;
                // indicate the object is being touched 
                touchedObject.GetComponent<Touchable>().Touch();
            }
        }

        /// <summary>
        /// Ends the touch of a touchedObject.
        /// </summary>
        public void EndTouch()
        {
            if (touchedObject != null)
            {
                // tell the object no PuppetHand is touching it
                touchedObject.GetComponent<Touchable>().puppetHandTouching = null;
                // indicate the object is not being touched
                touchedObject.GetComponent<Touchable>().Untouch();
            }

            // clear the touchObject
            touchedObject = null;
        }

        /// <summary>
        /// Initiates a grab on a grabbable object.
        /// </summary>
        public void Grab()
        {
            // if no object is currently grabbed
            if (grabbedObject == null)
            {
                // if an object touched is grabbable
                if (touchedObject != null && touchedObject.GetComponent<Grabbable>() && touchedObject.GetComponent<Grabbable>().isGrabbable)
                {
                    grabbedObject = touchedObject;
                }

                // if set to grab and entire stack of connected connectables
                if (grabAllConnected)
                {
                    if (touchedObject != null && touchedObject.GetComponent<Connectable>())
                    {
                        // search up through the hierarchy for a parent that is grabbable
                        GameObject grabbableAncestor = PuppetJumpManager.Instance.GetGrabbableAncestor(touchedObject.GetComponent<Connectable>().transform.gameObject);
                        if (grabbableAncestor != null)
                        {
                            grabbedObject = grabbableAncestor;
                        }
                    }
                }
            }

            if (grabbedObject != null)
            {
                grabbedObject.GetComponent<Grabbable>().SetAsGrabbed(this);
            }
        }

        /// <summary>
        /// Initiates the release of a grabbed object.
        /// </summary>
        public void Release()
        {
            if (grabbedObject != null)
            {
                grabbedObject.GetComponent<Grabbable>().Release(this);
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
    }
}
