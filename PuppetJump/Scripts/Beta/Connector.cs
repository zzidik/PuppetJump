#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using PuppetJump.Objs;
using PuppetJump.Utils;

namespace PuppetJump.Beta
{
    public class Connector : MonoBehaviour
    {
        public bool isUsable = true;                    // is the connector ready to be used
        public enum connectorType { male, female }
        public connectorType type;                      // male or female
        //[HideInInspector]
        public Connector attachedTo;                    // a compatible connector currently attached to (male and female)
        //[HideInInspector]
        public bool isConnected = false;                // indication that the connector is connected to a compatible connector (male and female)
        public int coupleID = 0;                        // a way to identify compatible connectors (male and female)
        //[HideInInspector]
        public Connector touching;                     // a compatible connector currently touching (male and female)
        public bool destroyRigidBodyOnConnect = true;   // on rare occasions you might want to keep the rigidbody after a connection, set to false if you want to keep it active
        //[HideInInspector]
        public Connectable connectable;                 // the connectable that this connector belongs to
        public GameObject ghost;                        // an object used to indicate where a connection can be made
        [HideInInspector]
        public bool ghostShowing = false;               // true if a ghost is currently displayed for this connector
        // <summary>
        /// A group of variables for a Button event.
        /// </summary>
        [System.Serializable]
        public class ConnectorEvents
        {
            public UnityEvent connect = new UnityEvent();           // triggers event(s) if button was pressed this frame
            public UnityEvent detach = new UnityEvent();            // trigger event(s) if button was released this frame
        }
        public ConnectorEvents connectorEvents;

        private void Start()
        {
            if (transform.parent.gameObject.GetComponent<Connectable>())
            {
                connectable = transform.parent.gameObject.GetComponent<Connectable>();
            }
            else
            {
                Debug.LogError("Connectors should be children of Connectables!");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (isUsable && !isConnected)
            {
                // if both objects in this collision are connectors
                if (GetComponent<Connector>() && other.GetComponent<Connector>() && other.GetComponent<Connector>().isUsable)
                {
                    // if female connectors are the glue of the hieracrhy
                    // and they match
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.matriarchal &&
                       other.GetComponent<Connector>().connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.matriarchal)
                    {
                        // male connectors handle the connecting and detaching
                        // if a male connector comes into contact with a female connector who share a coupleID
                        // and niether are currently connected to another connector
                        if (type == connectorType.male &&
                            other.GetComponent<Connector>().type == connectorType.female &&
                            coupleID == other.GetComponent<Connector>().coupleID &&
                            !isConnected &&
                            !other.GetComponent<Connector>().isConnected)
                        {

                            // let both connectors know which game object they are touching
                            // male
                            touching = other.gameObject.GetComponent<Connector>();
                            //female
                            other.GetComponent<Connector>().touching = this.gameObject.GetComponent<Connector>();

                            ConnectionCheck();
                        }
                    }

                    // if male connectors are the glue of the hieracrhy
                    // and they match
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.patriarchal &&
                       other.GetComponent<Connector>().connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.patriarchal)
                    {
                        // female connectors handle the connecting and detaching
                        // if a female connector comes into contact with a male connector who share a coupleID
                        // and niether are currently connected to another connector
                        if (type == connectorType.female &&
                            other.GetComponent<Connector>().type == connectorType.male &&
                            coupleID == other.GetComponent<Connector>().coupleID &&
                            !isConnected &&
                            !other.GetComponent<Connector>().isConnected)
                        {

                            // let both connectors know which game object they are touching
                            // female
                            touching = other.gameObject.GetComponent<Connector>();
                            //male
                            other.GetComponent<Connector>().touching = this.gameObject.GetComponent<Connector>();

                            ConnectionCheck();
                        }
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isUsable && !isConnected)
            {
                // if both objects in this collision are connectors
                if (GetComponent<Connector>() && other.GetComponent<Connector>() && other.GetComponent<Connector>().isUsable)
                {
                    // if female connectors are the glue of the hieracrhy
                    // and they match
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.matriarchal &&
                       other.GetComponent<Connector>().connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.matriarchal)
                    {
                        // male connectors handle the connecting and detaching
                        // if a male connector comes into contact with a female connector who share a coupleID
                        // and niether are currently connected to another connector
                        if (type == connectorType.male &&
                            other.GetComponent<Connector>().type == connectorType.female &&
                            coupleID == other.GetComponent<Connector>().coupleID &&
                            !isConnected &&
                            !other.GetComponent<Connector>().isConnected)
                        {

                            // let both connectors know which game object they are touching
                            // male
                            touching = other.gameObject.GetComponent<Connector>();
                            //female
                            other.GetComponent<Connector>().touching = this.gameObject.GetComponent<Connector>();

                            ConnectionCheck();
                        }
                    }

                    // if male connectors are the glue of the hieracrhy
                    // and they match
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.patriarchal &&
                       other.GetComponent<Connector>().connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.patriarchal)
                    {
                        // female connectors handle the connecting and detaching
                        // if a female connector comes into contact with a male connector who share a coupleID
                        // and niether are currently connected to another connector
                        if (type == connectorType.female &&
                            other.GetComponent<Connector>().type == connectorType.male &&
                            coupleID == other.GetComponent<Connector>().coupleID &&
                            !isConnected &&
                            !other.GetComponent<Connector>().isConnected)
                        {

                            // let both connectors know which game object they are touching
                            // female
                            touching = other.gameObject.GetComponent<Connector>();
                            //male
                            other.GetComponent<Connector>().touching = this.gameObject.GetComponent<Connector>();

                            ConnectionCheck();
                        }
                    }
                }

                
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (isUsable)
            {
                // if both objects in this collision are connectors
                if (GetComponent<Connector>() && other.GetComponent<Connector>() && other.GetComponent<Connector>().isUsable)
                {
                    // if female connectors are the glue of the hieracrhy
                    // and they match
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.matriarchal &&
                        other.GetComponent<Connector>().connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.matriarchal)
                    {
                        // male connectors handle the connecting and detaching
                        // if a male connector exits contact with a female connector who share a coupleID
                        if (type == connectorType.male &&
                        other.GetComponent<Connector>().type == connectorType.female &&
                        coupleID == other.GetComponent<Connector>().coupleID)
                        {
                            // let both connectors know they are not touching any compatible connectors
                            // male
                            touching = null;

                            // female
                            other.GetComponent<Connector>().touching = null;
                        }
                    }

                    // if male connectors are the glue of the hieracrhy
                    // and they match
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.patriarchal &&
                        other.GetComponent<Connector>().connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.patriarchal)
                    {
                        // female connectors handle the connecting and detaching
                        // if a female connector exits contact with a male connector who share a coupleID
                        if (type == connectorType.female &&
                        other.GetComponent<Connector>().type == connectorType.male &&
                        coupleID == other.GetComponent<Connector>().coupleID)
                        {
                            // let both connectors know they are not touching any compatible connectors
                            // female
                            touching = null;

                            // male
                            other.GetComponent<Connector>().touching = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes a connection between two connectors. 
        /// Sets the colliders of the connectables to ignor each other.
        /// May destroy the rigidbody of the newly connected connectable to keep only one rigidbody active in the hierarchy.
        /// Establishes the new heirarchy.
        /// </summary>
        void Connect()
        {
            // define the attached connectors
            attachedTo = touching;
            touching.attachedTo = this.gameObject.GetComponent<Connector>();

            // cancel any collision between the connectables
            if (connectable.GetComponent<Collider>() && attachedTo.connectable.GetComponent<Collider>())
            {
                Physics.IgnoreCollision(connectable.GetComponent<Collider>(), attachedTo.connectable.GetComponent<Collider>());
            }

            if (connectable.GetComponent<Rigidbody>())
            {
                // kill all velocity as a result of connection
                connectable.GetComponent<Rigidbody>().velocity = Vector3.zero;
                connectable.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                if (destroyRigidBodyOnConnect)
                {
                    // remove the rigidbody of the connectable being added to the heirarchy in the next frame
                    Destroy(connectable.GetComponent<Rigidbody>()); 
                }
                
            }

            if (attachedTo.connectable.GetComponent<Rigidbody>())
            {
                // kill all velocity as a result of connection
                attachedTo.connectable.GetComponent<Rigidbody>().velocity = Vector3.zero;
                attachedTo.connectable.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            
            // create the hierarchy
            connectable.transform.parent = attachedTo.connectable.transform;
            // position the child
            connectable.transform.localPosition = attachedTo.transform.localPosition;
            connectable.transform.localEulerAngles = attachedTo.transform.localEulerAngles;

            if (connectable.GetComponent<Touchable>())
            {
                PuppetHand ph = connectable.GetComponent<Touchable>().puppetHandTouching;
                if(ph != null)
                {
                    ph.EndTouch();
                }
            }

            // indicate the connectors are connected to a compatible connector
            isConnected = true;
            attachedTo.isConnected = true;

            //Debug.Log("Connection between " + connectable.gameObject.name + " and " + attachedTo.connectable.gameObject.name + " made.");

            // if there is a Connectable Manager in the scene
            if (ConnectableManager.Instance)
            {
                // adjust the masses of all connectables
                ConnectableManager.Instance.CheckMasses();
            }

            connectorEvents.connect.Invoke();
            attachedTo.connectorEvents.connect.Invoke();
        }

        

        public void Detach()
        {
            if (attachedTo != null)
            {
                attachedTo.isConnected = false;
                attachedTo.attachedTo = null;

                // enable any collision between the parents
                // this was removed when the connection was made
                if (connectable.GetComponent<Collider>() && attachedTo.connectable.GetComponent<Collider>())
                {
                    Physics.IgnoreCollision(connectable.GetComponent<Collider>(), attachedTo.connectable.GetComponent<Collider>(), false);
                }

                // reset the center of mass
                if (connectable.GetComponent<Rigidbody>())
                {
                    connectable.GetComponent<Rigidbody>().ResetCenterOfMass();
                }
                if (attachedTo.connectable.GetComponent<Rigidbody>())
                {
                    attachedTo.connectable.GetComponent<Rigidbody>().ResetCenterOfMass();
                }

                connectorEvents.detach.Invoke();
                attachedTo.connectorEvents.detach.Invoke();

                touching = null;
                attachedTo.touching = null;

                //Debug.Log("Detach between " + connectable.gameObject.name + " and " + attachedTo.connectable.gameObject.name + " made.");

                attachedTo = null;
                isConnected = false;

                // if there is a Connectable Manager in the scene
                if (ConnectableManager.Instance)
                {
                    // adjust the masses of all connectables
                    ConnectableManager.Instance.CheckMasses();
                }
            }
        }

        /// <summary>
        /// While a connector is not attached but is touching another connector,
        /// this checks to see if either of the Connectables are currently grabbed.
        /// If neither are grabbed, the connection is made.
        /// </summary>
        public void ConnectionCheck()
        {

            // if the connectors that are touching are not attached
            if (attachedTo == null && touching != null)
            {
                // a check to make sure neither of the two connectables are currently grabbed
                bool noneGrabbed = true;

                bool thisIsGrabbed = false;
                bool touchingIsGrabbed = false;

                if (connectable.GetComponent<Grabbable>() && connectable.GetComponent<Grabbable>().isGrabbed)
                {
                    noneGrabbed = false;
                    thisIsGrabbed = true;
                }

                Connectable touchingConnectable = touching.connectable;
                if (touchingConnectable.GetComponent<Grabbable>() && touchingConnectable.GetComponent<Grabbable>().isGrabbed)
                {
                    noneGrabbed = false;
                    touchingIsGrabbed = true;
                }

                // search the hierarchies for parents that might be grabbed
                // search the hierarchy of this connectable
                List<GameObject> ancestors = PuppetJumpManager.Instance.GetAncestors(connectable.transform.gameObject);
                int numAncestors = ancestors.Count;
                for (int a = 0; a < numAncestors; a++)
                {
                    if (ancestors[a].GetComponent<Grabbable>() && ancestors[a].GetComponent<Grabbable>().isGrabbed)
                    {
                        noneGrabbed = false;
                        thisIsGrabbed = true;
                        return;
                    }
                }
                // search the hierarchy of touching connectable
                ancestors = PuppetJumpManager.Instance.GetAncestors(touchingConnectable.transform.gameObject);
                numAncestors = ancestors.Count;
                for (int a = 0; a < numAncestors; a++)
                {
                    if (ancestors[a].GetComponent<Grabbable>() && ancestors[a].GetComponent<Grabbable>().isGrabbed)
                    {
                        noneGrabbed = false;
                        touchingIsGrabbed = true;
                        return;
                    }
                }

                //Debug.Log(touchingIsGrabbed +"," + thisIsGrabbed);

                // if neither connectable is grabbed
                if (noneGrabbed)
                {
                    // if the family structure is matriarchal
                    // let the male establish the connection, just so they both aren't at the same time
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.matriarchal && type == connectorType.male)
                    {
                        // make a connection
                        Connect();
                    }

                    // if the family structure is patriarchal
                    // let the female establish the connection, just so they both aren't at the same time
                    if (connectable.GetComponent<Connectable>().hierarchalStructure == Connectable.HierarchalStructures.patriarchal && type == connectorType.female)
                    {
                        // make a connection
                        Connect();
                    }
                }
                /*
                else if(thisIsGrabbed && !touchingIsGrabbed)
                {
                   touching.Connect();
                }
                else if(!thisIsGrabbed && touchingIsGrabbed)
                {
                    Connect();
                }
                */
            }
        }

        /*
        public void DetachCheck()
        {
            Debug.Log("Detach Check Called");
            // if the connectors that are touching are not attached
            if (attachedTo != null)
            {
                bool touchingIsGrabbedOrUngrabbale = false;

                Connectable touchingConnectable = attachedTo.connectable;
                if (touchingConnectable.GetComponent<Grabbable>() && touchingConnectable.GetComponent<Grabbable>().isGrabbed)
                {
                    touchingIsGrabbedOrUngrabbale = true;
                }

                // search the hierarchies for parents that might be grabbed
                // search the hierarchy of this connectable
                List<GameObject> ancestors = PuppetJumpManager.Instance.GetAncestors(touchingConnectable.transform.gameObject);
                int numAncestors = ancestors.Count;
                for (int a = 0; a < numAncestors; a++)
                {
                    if (ancestors[a].GetComponent<Grabbable>() && ancestors[a].GetComponent<Grabbable>().isGrabbed)
                    {
                        touchingIsGrabbedOrUngrabbale = true;
                        return;
                    }
                }

                if (!touchingConnectable.GetComponent<Grabbable>())
                {
                    touchingIsGrabbedOrUngrabbale = true;
                }
                

                if (touchingIsGrabbedOrUngrabbale)
                {
                    Detach();
                }
            }
        }
        */

#if UNITY_EDITOR
        // label with connector information in scene editor
        public void DrawGizmo()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleCenter;

            if (type == connectorType.male)
            {
                style.normal.textColor = Color.cyan;
                Handles.Label(transform.position, "Male Connector Child (" + coupleID.ToString() + ")", style);
            }
            else if (type == connectorType.female)
            {
                style.normal.textColor = Color.magenta;
                Handles.Label(transform.position, "Female Connector Child (" + coupleID.ToString() + ")", style);
            }
        }
#endif

    }
}
