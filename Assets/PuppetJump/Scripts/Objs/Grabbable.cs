using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PuppetJump.OVR;
using PuppetJump.Utils;
using PuppetJump.Beta;

namespace PuppetJump.Objs
{
    public class Grabbable : Touchable
    {
        [ReadOnly]
        public Rigidbody rigidbody;
        public enum GrabStyles { child, fixedJoint };
        public GrabStyles grabStyle;
        [ReadOnly]
        public bool isGrabbed = false;                                      // state of grabbed object
        public bool isGrabbable = true;                                     // can the object be grabbed
        public bool isThrowable = false;                                    // determines if the object can be thrown
        public float grabBreakforce = Mathf.Infinity;                       // amount of force to break the object from the grab
        public GameObject attach;                                           // a seperate attach point game object, used to orient a grabbable object into a specific position relative to the controller when grabbed
        [ReadOnly]
        public List<PuppetHand> puppetHandsGrabbing = new List<PuppetHand>();
        [HideInInspector]
        public Transform originalParent;                                    // store the starting parent of the object, cause it might change when it is grabbed
        [System.Serializable]
        public class GrabEvents
        {
            public UnityEvent grabbed = new UnityEvent();                   // triggers event(s) if object is grabbed this frame
            public UnityEvent released = new UnityEvent();                  // trigger event(s) if object is released from a grab this frame
        }
        public GrabEvents grabEvents;
        public class RigidBodyProperties
        {
            public float mass = 1.0f;
            public bool useGravity = true;
            public bool isKinematic = false;
        }
        [HideInInspector]
        public RigidBodyProperties rbProperties = new RigidBodyProperties();   // store the properties of original rigid body
        [ReadOnly]
        public bool hasRigidBodyAtStart = false;                            // does this object have a rigidbody to start
        public bool grabHidesPuppetHandDisplay = false;                     // hides the PuppetHand displayObject when grabbing if true


        private void Awake()
        {
            if (GetComponent<Rigidbody>())
            {
                rigidbody = GetComponent<Rigidbody>();
                // store orginal rigidbody properties
                // becasue the rigidbody will be removed if it is a child grabbed
                rbProperties.mass = rigidbody.mass;
                rbProperties.useGravity = rigidbody.useGravity;
                rbProperties.isKinematic = rigidbody.isKinematic;
                hasRigidBodyAtStart = true;
            }

            // store the original parent
            originalParent = transform.parent;
        }

        public void SetAsGrabbed(PuppetHand grabber)
        {
            switch (grabStyle)
            {
                case GrabStyles.child:

                    // if already being grabbed
                    if (isGrabbed)
                    {
                        // when child grabbed only one PuppetHand can grab at once
                        Release(puppetHandsGrabbing[0]);
                    }

                    StopAndPositiionForGrab(grabber);

                    // parent the grabbedObject to the grabber
                    transform.parent = grabber.transform;

                    // if a rigidbody is present
                    if (rigidbody != null)
                    {
                        // add the mass to the grabber
                        grabber.rigidbody.mass += rigidbody.mass;

                        // remove the rigidbody from the grabbed object
                        Destroy(rigidbody);
                        rigidbody = null;
                    }
                    else
                    {
                        // if the grabbed object is a connectable
                        // the rigidbody may have been removed during it's connection
                        if (GetComponent<Connectable>())
                        {
                            // if it had a rigidbody at the start
                            if (hasRigidBodyAtStart)
                            {
                                // add the mass to the grabber
                                grabber.rigidbody.mass += rbProperties.mass;
                            }
                        }
                    }

                    EstablishObjectAsGrabbed(grabber);

                    // if the grab object is a connectable
                    if (GetComponent<Connectable>())
                    {
                        Connectable connectable = GetComponent<Connectable>();
                        HandleGrabbedConnectable(connectable);
                    }
                    break;

                case GrabStyles.fixedJoint:
                    
                    if (grabber.puppetString.grabJointFixed == null)
                    {
                        StopAndPositiionForGrab(grabber);

                        // add a fixed joint to the grabber's string
                        grabber.puppetString.grabJointFixed = grabber.puppetString.gameObject.AddComponent<FixedJoint>();

                        // connect this object 
                        grabber.puppetString.grabJointFixed.connectedBody = rigidbody;

                        // the amount of force it will take to break this joint
                        // for example when the object comes into contact with another object, like a table
                        grabber.puppetString.grabJointFixed.breakForce = grabBreakforce;

                        EstablishObjectAsGrabbed(grabber);
       
                        // if the grab object is a connectable
                        if (GetComponent<Connectable>())
                        {
                            Connectable connectable = GetComponent<Connectable>();
                            HandleGrabbedConnectable(connectable);
                        }
                    }
                    break;
            }

            if (grabHidesPuppetHandDisplay)
            {
                if(grabber.puppetHandDisplayManager != null)
                {
                    grabber.puppetHandDisplayManager.HideDisplay(true);
                }
                
                if(grabber.displayObject != null)
                {
                    grabber.displayObject.SetActive(false);
                }
            }
        }

        void StopAndPositiionForGrab(PuppetHand grabber)
        {
            if (rigidbody != null)
            {
                // set the velocity to zero to stop all movement of the object when grabbed
                rigidbody.velocity = Vector3.zero;
            }

            // if this object has a seperate attach point object
            if (attach != null)
            {
                // an empty game object used to aid in positioning
                GameObject dummy = GetDummy();
                dummy.transform.position = attach.transform.position;
                dummy.transform.rotation = attach.transform.rotation;

                // reparent object to dummy
                transform.parent = dummy.transform;

                // move the dummy to the position of the hand
                dummy.transform.position = grabber.transform.position;
                dummy.transform.rotation = grabber.transform.rotation;

                // reparent the object to it's original parent
                transform.parent = originalParent;

                // remove the dummy
                DestroyImmediate(dummy);
            }
        }

        void EstablishObjectAsGrabbed(PuppetHand grabber)
        {
            // set the object state to grabbed
            isGrabbed = true;

            // tell the grabbed object which grabber is grabbing it
            puppetHandsGrabbing.Add(grabber);

            // clear the touched object
            grabber.touchedObject = null;

            // ignore collisions between object and characterController
            Physics.IgnoreCollision(GetComponent<Collider>(), PuppetJumpManager.Instance.puppetRig.characterController);

            // set the object to untouched
            Untouch();

            // invoke grabbed events for object
            grabEvents.grabbed.Invoke();
        }

        void HandleGrabbedConnectable(Connectable connectable)
        {
            // go through it's list of connectors
            int numConnectors = connectable.connectors.Count;
            for (int c = 0; c < numConnectors; c++)
            {
                // if the female connectors are the glue of the family
                if (connectable.hierarchalStructure == Connectable.HierarchalStructures.matriarchal)
                {
                    // detach just the male connectors from their females
                    if (connectable.connectors[c].type == Connector.connectorType.male && connectable.connectors[c].isConnected)
                    {
                        connectable.connectors[c].Detach();
                    }
                }

                // if the male connectors are the glue of the family
                if (connectable.hierarchalStructure == Connectable.HierarchalStructures.patriarchal)
                {
                    // detach just the female connectors from their males
                    if (connectable.connectors[c].type == Connector.connectorType.female && connectable.connectors[c].isConnected)
                    {
                        connectable.connectors[c].Detach();
                    }
                }
            }

            // if there is a Connectable Manager in the scene
            if (ConnectableManager.Instance)
            {
                // if the ghosts for this object are not showing
                if (ConnectableManager.Instance.ghostShowingForThis != connectable)
                {
                    ConnectableManager.Instance.SearchForGhosts(connectable);
                }
            }
        }

        /// <summary>
        /// Creates and empty gameObject used to help in posiitoning.
        /// </summary>
        /// <returns></returns>
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

        public void Release(PuppetHand grabber)
        {
            switch (grabStyle)
            {
                case GrabStyles.child:
                    // return to orginal parent
                    transform.parent = originalParent;

                    if (hasRigidBodyAtStart)
                    {
                        // restore the rigidbody
                        rigidbody = this.gameObject.AddComponent<Rigidbody>();
                        rigidbody.mass = rbProperties.mass;
                        rigidbody.useGravity = rbProperties.useGravity;
                        rigidbody.isKinematic = rbProperties.isKinematic;

                        // set the grabber back to it's orginal mass
                        grabber.rigidbody.mass = grabber.originalMass;
                    }
                    break;
                case GrabStyles.fixedJoint:
                    if (grabber.puppetString.grabJointFixed != null)
                    {
                        // destroy the joint between the grabber and the object
                        Destroy(grabber.puppetString.grabJointFixed);
                        grabber.puppetString.grabJointFixed = null;
                    }
                    break;
            }

            // THROW
            // check if object is throwable
            if (rigidbody != null && isThrowable)
            {
                rigidbody.AddForce(grabber.rigidbody.velocity, ForceMode.Impulse);
            }

            Connectable connectable = null;
            // if the grab object is a connectable
            if (GetComponent<Connectable>())
            {
                connectable = GetComponent<Connectable>();

                // if there is a Connectable Manager in the scene
                if (ConnectableManager.Instance)
                {
                    // if ghost are currently showing for this object
                    if (ConnectableManager.Instance.ghostShowingForThis == connectable)
                    {
                        ConnectableManager.Instance.ClearAllGhosts();
                    }
                }
            }

            // enable collisions between object and characterController
            Physics.IgnoreCollision(GetComponent<Collider>(), PuppetJumpManager.Instance.puppetRig.characterController, false);

            // remove grabber from list
            puppetHandsGrabbing.Remove(grabber);

            if(puppetHandsGrabbing.Count == 0)
            {
                // set the object state to not grabbed
                isGrabbed = false;
            }

            // if the grab object is a connectable
            if (connectable != null)
            {
                // check if a connection is made by letting go of this object
                int numConnectors = connectable.connectors.Count;
                for (int c = 0; c < numConnectors; c++)
                {
                    connectable.connectors[c].ConnectionCheck();
                }
            }

            if (puppetHandsGrabbing.Count == 0)
            {
                // invoke released events for object
                grabEvents.released.Invoke();
            }

            // empty the grabbed object
            grabber.grabbedObject = null;

            if (grabHidesPuppetHandDisplay)
            {
                if (grabber.puppetHandDisplayManager != null)
                {
                    grabber.puppetHandDisplayManager.HideDisplay(false);
                }
                
                if (grabber.displayObject != null)
                {
                    grabber.displayObject.SetActive(true);
                }
            }
        }
    }
}
