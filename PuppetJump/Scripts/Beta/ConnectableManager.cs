using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Objs;


namespace PuppetJump.Beta
{
    public class ConnectableManager : MonoBehaviour
    {
        private static ConnectableManager _instance = null;                                 // will hold a single instance of this class
        public static ConnectableManager Instance { get { return _instance; } }             // returns the instance of this class
        private List<Connectable> connectableChildren = new List<Connectable>();            // a list to hold all connectables in a heirarchy
        public List<Connectable> connectables = new List<Connectable>();                    // a list of all connectables in the scene
        private List<GameObject> ghosts = new List<GameObject>();                           // a list of ghost gameobject currently being show
        [HideInInspector]
        public Connectable ghostShowingForThis;                                             // a connectable for which a search for ghosts has been preformed and are showing if available
        public List<Connectable> spawnables = new List<Connectable>();                      // a list of connectables that can be spawned into the scene by connectors

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
        }

        /// <summary>
        /// Searches for ghosts that can appear to show available places for the connectable to be placed.
        /// </summary>
        /// <param name="connectable">The connectable that is looking for connections.</param>
        public void SearchForGhosts(Connectable cLookingForGhosts)
        {
            // how many connectables are in the scene
            int numConnectables = connectables.Count;
            // how many connectors does this connectable have
            int numConnectors = cLookingForGhosts.connectors.Count;
            // go through the list of connectables
            for (int a = 0; a < numConnectables; a++)
            {
                // skip looking for ghosts on the connectable looking for ghosts
                if(connectables[a] != cLookingForGhosts)
                {
                    // if their family structures are the same
                    if(connectables[a].hierarchalStructure == cLookingForGhosts.hierarchalStructure)
                    {
                        // how many connectors on the connectable in the list
                        int numConnectorsOnListItem = connectables[a].connectors.Count;
                        // check all of the connectors of the connectable in the list
                        for (int b = 0; b < numConnectorsOnListItem; b++)
                        {
                            // check all of the connectors of the connectable looking for ghosts
                            for (int c = 0; c < numConnectors; c++)
                            {
                                // if both are active
                                if (cLookingForGhosts.connectors[c].isUsable && connectables[a].connectors[b].isUsable)
                                {
                                    // if both are not connected
                                    if (!cLookingForGhosts.connectors[c].isConnected && !connectables[a].connectors[b].isConnected)
                                    {
                                        // if coupleIDs match
                                        if (cLookingForGhosts.connectors[c].coupleID == connectables[a].connectors[b].coupleID)
                                        {
                                            // and types are opposite
                                            if (cLookingForGhosts.connectors[c].type != connectables[a].connectors[b].type)
                                            {
                                                // if the connector has a ghost
                                                if (connectables[a].connectors[b].ghost != null)
                                                {
                                                    // create a ghost
                                                    CreateConnectorGhost(connectables[a].connectors[b]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // indicate which connectable we last search for ghost for
            ghostShowingForThis = cLookingForGhosts;
        }

        /// <summary>
        /// Creates a ghost object from the one listed by the connector.
        /// Shows it at the connector's position.
        /// Adds it to the ghosts on display list.
        /// </summary>
        /// <param name="c"></param>
        void CreateConnectorGhost(Connector c)
        {
            // if no ghost is currently displayed for this connector
            if (!c.ghostShowing)
            {
                GameObject newGhost = GameObject.Instantiate(c.ghost, Vector3.zero, Quaternion.identity, c.transform);
                newGhost.transform.localPosition = Vector3.zero;
                newGhost.transform.localEulerAngles = Vector3.zero;
                newGhost.name = "ghost";
                c.ghostShowing = true;
                ghosts.Add(newGhost);
            }
        }

        /// <summary>
        /// Spawns a new connectable prefab into the see on a connector.
        /// </summary>
        /// <param name="c"></param>
        public void Spawn(Connector c)
        {
            // find a coupleID match in the list of spawnables
            int numSpawnables = spawnables.Count;
            for(int s = 0; s < numSpawnables; s++)
            {
                // check all connectors of the spawnable connectable
                int numConnectors = spawnables[s].connectors.Count;
                for(int cn = 0; cn < numConnectors; cn++)
                {
                    // if there is a coupleID match
                    if(spawnables[s].connectors[cn].coupleID == c.coupleID)
                    {
                        // spawn a new connectable into the scene
                        GameObject prefab = spawnables[s].gameObject;
                        GameObject newPart = GameObject.Instantiate(prefab, c.transform.position, c.transform.rotation);
                        newPart.name = prefab.name;
                    }
                }
            }
        }

        /// <summary>
        /// Destorys all ghosts in the list and clears the list.
        /// </summary>
        public void ClearAllGhosts()
        {
            // destroyy all ghots obbjects in the scene
            int numGhosts = ghosts.Count;
            for(int g = 0; g < numGhosts; g ++)
            {
                Destroy(ghosts[g]);
            }
            // empty the list
            ghosts.Clear();

            // how many connectables are in the scene
            int numConnectables = connectables.Count;
            // go through the list of connectables
            for (int a = 0; a < numConnectables; a++)
            {
                // how many connectors on the connectable in the list
                int numConnectorsOnListItem = connectables[a].connectors.Count;
                // check all of the connectors of the connectable in the list
                for (int b = 0; b < numConnectorsOnListItem; b++)
                {
                    // indictae there are no ghosts showing for any connectors
                    connectables[a].connectors[b].ghostShowing = false;
                }
            }

            // indicate there are no ghost showing for any object
            ghostShowingForThis = null;
        }

        /// <summary>
        /// Whenever a connection or a detach happens,
        /// this makes sure masses of connectables in the scene
        /// are set properly.
        /// </summary>
        public void CheckMasses()
        {
            int numConnectables = connectables.Count;
            for(int con = 0; con < numConnectables; con++)
            {
                // if the connectable has a rigidbody at start
                if(connectables[con].startMass != -1f)
                {
                    // if this connectable currently has a rigidbody
                    if (connectables[con].gameObject.GetComponent<Rigidbody>())
                    {
                        // give it its proper mass
                        connectables[con].gameObject.GetComponent<Rigidbody>().mass = TotalMass(connectables[con]);
                    }
                }
            }
        }

        /// <summary>
        /// Searches all the children of a connectable object for other connectables.
        /// </summary>
        /// <param name="parent">The transform of the connectable gameobject.</param>
        /// <param name="list">The list of connectable children.</param>
        private void GetConnectableChildren(Transform parent, List<Connectable> list)
        {
            foreach (Transform child in parent)
            {
                if (child.GetComponent<Connectable>())
                {
                    list.Add(child.GetComponent<Connectable>());
                }
                GetConnectableChildren(child, list);
            }
        }

        /// <summary>
        /// Gets the proper mass of a connectable object.
        /// </summary>
        /// <param name="con">The connectable to find the mass of.</param>
        /// <returns></returns>
        private float TotalMass(Connectable con)
        {
            // the starting mass of the connectable
            float totalMass = con.startMass;

            // get a list of all children who are connectables
            connectableChildren = new List<Connectable>();
            GetConnectableChildren(con.gameObject.transform, connectableChildren);

            // go through that list
            int numConChildren = connectableChildren.Count;
            for(int c = 0; c < numConChildren; c++)
            {
                // if the child connectables add mass to the parent
                if (connectableChildren[c].addMassToParent)
                {
                    totalMass += connectableChildren[c].startMass;
                }
            }

            // empty list
            connectableChildren.Clear();
            connectableChildren = null;

            return totalMass;
        }
    }
}

