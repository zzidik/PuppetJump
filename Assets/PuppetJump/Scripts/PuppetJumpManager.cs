using UnityEngine;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using PuppetJump.Objs;
using PuppetJump.Utils;
using PuppetJump.OVR;

namespace PuppetJump
{
    public class PuppetJumpManager : MonoBehaviour
    {
        private static PuppetJumpManager _instance = null;                              // will hold a single instance of this class
        public static PuppetJumpManager Instance { get { return _instance; } }          // returns the instance of this class
        public bool autoType = false;                                                   // true if the application determines which play type is active
        public enum DeviceTypes { Desktop, SteamVR, OVR };                              // a list of possible play types supported by PuppetJump
        public DeviceTypes deviceType;
        public int ignoreCollisionsLayer = 8;                                           // index of the ignore collisions layer. Set up in the Physics Manager
        public float renderScale = 1.5f;                                                // controls the actual size of eye textures as a multiplier of the device's default resolution
        public CameraRig cameraRig;                                                     // the active camera rig
        public InputDevice rightHandVRInputDevice;                                      // the right hand VR controller
        public InputDevice leftHandVRInputDevice;                                       // the left hand VR controller
        [ReadOnly]
        public bool vrReady = false;                                                    // indicates that a camera rig and two controllers are active and ready for use
        public PuppetRig puppetRig;
        
  
        void Awake()
        {
            // insure this object class is a singleton
            // if there is already an instance of this class
            // and it is not this object
            if (_instance != null && _instance != this)
            {
                // get rid of any other instance
                Destroy(this.gameObject);
            }
            else
            {
                // make this the single instance
                _instance = this;
                // keep in all scenes
                DontDestroyOnLoad(this);
            }

            if (UnityEngine.XR.XRDevice.isPresent)
            {
                // set size of eye textures
                UnityEngine.XR.XRSettings.eyeTextureResolutionScale = renderScale;
            }
        }

        /// <summary>
        /// When the scene is loaded adds OnSceneLoaded as a delegate to run.
        /// </summary>
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if(cameraRig != null && rightHandVRInputDevice != null && leftHandVRInputDevice != null)
            {
                vrReady = true;
            }
            else
            {
                vrReady = false;
            }
        }

        /// <summary>
        /// After the scene has loaded, if the application determines which device is in use,
        /// PuppetJump activates the cooresponding camera rig and deactivates the unneeded ones.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // if using automatic detection of connected device
            if (autoType)
            {
                // if a vr device is present
                if (UnityEngine.XR.XRDevice.isPresent)
                {
                    ActivatVRPuppetRig();

                    // get the model of the XR device connected
                    string model = UnityEngine.XR.XRDevice.model != null ? UnityEngine.XR.XRDevice.model : "";
                    Debug.Log(model);

                    // if an Oculus device is detected
                    if (model.ToLower().Contains("oculus"))
                    {
                        ActivateOVRCameraRig();
                    }
                    // if a non-Oculus device is detected
                    else
                    {
                        ActivateSteamVRCameraRig();
                    }
                }

                // if no vr device is present
                else
                {
                    ActivateDestopPuppetRig();
                    ActivateDesktopCameraRig();
                }

            // if not using automatic detection of connected device
            }
            else
            {
                switch (deviceType)
                {
                    case DeviceTypes.Desktop:
                        ActivateDestopPuppetRig();
                        ActivateDesktopCameraRig();
                        break;
                    case DeviceTypes.SteamVR:
                        ActivatVRPuppetRig();
                        ActivateSteamVRCameraRig();
                        break;
                    case DeviceTypes.OVR:
                        ActivatVRPuppetRig();
                        ActivateOVRCameraRig();
                        break;
                }
            }
        }

        void ActivateDestopPuppetRig()
        {
            if (GameObject.Find("PuppetRigDesktop"))
            {
                puppetRig = GameObject.Find("PuppetRigDesktop").GetComponent<PuppetRig>();
                puppetRig.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Please place the DPuppetRigDesktop prefab in your scene.");
            }

            if (GameObject.Find("PuppetRigVR"))
            {
                GameObject.Find("PuppetRigVR").SetActive(false);
            }
        }

        void ActivatVRPuppetRig()
        {
            if (GameObject.Find("PuppetRigVR"))
            {
                puppetRig = GameObject.Find("PuppetRigVR").GetComponent<PuppetRig>();
                puppetRig.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Please place the PuppetRigVR prefab in your scene.");
            }

            if (GameObject.Find("PuppetRigDesktop"))
            {
                GameObject.Find("PuppetRigDesktop").SetActive(false);
            }
        }

        /// <summary>
        /// Activates the DesktopCameraRig and deactivates the others.
        /// </summary>
        void ActivateDesktopCameraRig()
        {
            // if the SteamVRCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("SteamVRCameraRig [PuppetJump]"))
            {
                // deactivate it
                GameObject.Find("SteamVRCameraRig [PuppetJump]").SetActive(false);
            }

            // if the OVRCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("OVRCameraRig [PuppetJump]"))
            {
                // deactivate it
                GameObject.Find("OVRCameraRig [PuppetJump]").SetActive(false);
            }

            // if the DesktopCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("DesktopCameraRig [PuppetJump]"))
            {
                // activate it
                cameraRig = GameObject.Find("DesktopCameraRig [PuppetJump]").GetComponent<CameraRig>();
                cameraRig.gameObject.SetActive(true);
            }
            // if the DesktopCameraRig [PuppetJump] is not in the scene
            else
            {
                // throw error
                Debug.LogError("Please place the DesktopCameraRig [PuppetJump] prefab in your scene.");
            }

            deviceType = PuppetJumpManager.DeviceTypes.Desktop;
        }

        /// <summary>
        /// Activates the OVRCameraRig and deactivates the SteamVRCameraRig.
        /// </summary>
        void ActivateOVRCameraRig()
        {
            // if the DesktopCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("DesktopCameraRig [PuppetJump]"))
            {
                // deactivate it
                GameObject.Find("DesktopCameraRig [PuppetJump]").SetActive(false);
            }

            // if the SteamVRCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("SteamVRCameraRig [PuppetJump]"))
            {
                // deactivate it
                GameObject.Find("SteamVRCameraRig [PuppetJump]").SetActive(false);
            }

            // if the OVRCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("OVRCameraRig [PuppetJump]"))
            {
                // activate it
                cameraRig = GameObject.Find("OVRCameraRig [PuppetJump]").GetComponent<CameraRig>();
                cameraRig.gameObject.SetActive(true);
            }
            // if the OVRCameraRig [PuppetJump] is not in the scene
            else
            {
                // throw error
                Debug.LogError("Please place the OVRCameraRig [PuppetJump] prefab in your scene.");
            }

            deviceType = PuppetJumpManager.DeviceTypes.OVR;
        }

        /// <summary>
        /// Activates the SteamVRCameraRig and deactivates the OVRCameraRig.
        /// </summary>
        void ActivateSteamVRCameraRig()
        {
            // if the DesktopCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("DesktopCameraRig [PuppetJump]"))
            {
                // deactivate it
                GameObject.Find("DesktopCameraRig [PuppetJump]").SetActive(false);
            }

            // if the OVRCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("OVRCameraRig [PuppetJump]"))
            {
                // deactivate it
                GameObject.Find("OVRCameraRig [PuppetJump]").SetActive(false);
            }

            // if the SteamVRCameraRig [PuppetJump] is in the scene
            if (GameObject.Find("SteamVRCameraRig [PuppetJump]"))
            {
                // activate it
                cameraRig = GameObject.Find("SteamVRCameraRig [PuppetJump]").GetComponent<CameraRig>();
                cameraRig.gameObject.SetActive(true);
            }
            // if the SteamVRCameraRig [PuppetJump] is not in the scene
            else
            {
                // throw error
                Debug.LogError("Please place the SteamVRCameraRig [PuppetJump] prefab in your scene.");
            }

            deviceType = PuppetJumpManager.DeviceTypes.SteamVR;
        }

        /// <summary>
        /// Removes OnSceneLoaded delegate when scene is changed.
        /// </summary>
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Gets a list of a gameobject's ancestors (parents) all the way up to the world.
        /// </summary>
        /// <param name="go">The gameobject to start from.</param>
        /// <returns>A list of gameobjects.</returns>
        public List<GameObject> GetAncestors(GameObject go)
        {
            List<GameObject> ancestors = new List<GameObject>();
            Transform ancestor = go.transform.parent;
            while (ancestor != null)
            {
                ancestors.Add(ancestor.gameObject);
                ancestor = ancestor.parent;
            }

            return ancestors;
        }

        /// <summary>
        /// Gets a list of a gameobject's children.
        /// </summary>
        /// <param name="go">The gameobject to start from.</param>
        /// <returns>A list of gameobjects.</returns>
        public List<GameObject> GetChildren(GameObject go)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in go.transform)
            {
                children.Add(child.gameObject);
            }

            return children;
        }

        /// <summary>
        /// Searches up the heiracrhy to find an ancestor that is touchable.
        /// </summary>
        /// <param name="go">The start object to search up from.</param>
        /// <returns>A touchable ancestor or null.</returns>
        public GameObject GetTouchableAncestor(GameObject go)
        {
            GameObject touchableAncestor = null;

            List<GameObject> ancestors = GetAncestors(go);

            int numAncestors = ancestors.Count;
            for (int a = 0; a < numAncestors; a++)
            {
                if (ancestors[a].GetComponent<Touchable>())
                {
                    if (ancestors[a].GetComponent<Touchable>().isTouchable)
                    {
                        touchableAncestor = ancestors[a];
                        break;
                    }
                }
            }

            return touchableAncestor;
        }

        /// <summary>
        /// Searches up the heiracrhy to find an ancestor that is pushable.
        /// </summary>
        /// <param name="go">The start object to search up from.</param>
        /// <returns>A pushable ancestor or null.</returns>
        public GameObject FindPushableAncestor(GameObject go)
        {
            GameObject pushableAncestor = null;

            List<GameObject> ancestors = GetAncestors(go);
            int numAncestors = ancestors.Count;
            for (int a = 0; a < numAncestors; a++)
            {
                if (ancestors[a].GetComponent<Touchable>() && ancestors[a].GetComponent<Touchable>().isTouchable && ancestors[a].GetComponent<Rigidbody>())
                {
                    pushableAncestor = ancestors[a];
                    break;
                }
            }

            return pushableAncestor;
        }

        /// <summary>
        /// Searches up the heiracrhy to find an ancestor that is grabbable.
        /// </summary>
        /// <param name="go">The start object to search up from.</param>
        /// <returns>A grabbable ancestor or null.</returns>
        public GameObject GetGrabbableAncestor(GameObject go)
        {
            GameObject grabbableAncestor = null;

            List<GameObject> ancestors = GetAncestors(go);
            int numAncestors = ancestors.Count;
            for (int a = 0; a < numAncestors; a++)
            {
                if (ancestors[a].GetComponent<Grabbable>() && ancestors[a].GetComponent<Grabbable>().isGrabbable)
                {
                    grabbableAncestor = ancestors[a];
                }
            }

            return grabbableAncestor;
        }

        /// <summary>
        /// Searches up the heiracrhy to find an ancestor that has a rigidbody.
        /// </summary>
        /// <param name="go">The start object to search up from.</param>
        /// <returns>An ancestor with a rigidbody or null.</returns>
        public GameObject GetRigidBodyAncestor(GameObject go)
        {
            GameObject rbAncestor = null;

            List<GameObject> ancestors = GetAncestors(go);
            int numAncestors = ancestors.Count;
            for (int a = 0; a < numAncestors; a++)
            {
                if (ancestors[a].GetComponent<Rigidbody>())
                {
                    rbAncestor = ancestors[a];
                }
            }

            return rbAncestor;
        }

        /// <summary>
        /// Sets the layer of an object and all of it's children.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="layer"></param>
        public void SetChildLayers(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                child.gameObject.layer = layer;
                SetChildLayers(child, layer);
            }
        }

        /// <summary>
        /// Sets the rigidbody of an object, and those of all it's children, to kinematic.
        /// </summary>
        /// <param name="t"></param>
        public void SetAllRigidBodiesToKinematic(Transform t)
        {
            if (t.GetComponent<Rigidbody>())
            {
                t.GetComponent<Rigidbody>().isKinematic = true;
            }
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                if (child.GetComponent<Rigidbody>())
                {
                    child.GetComponent<Rigidbody>().isKinematic = true;
                }
                SetAllRigidBodiesToKinematic(child);
            }
        }
    }
}


