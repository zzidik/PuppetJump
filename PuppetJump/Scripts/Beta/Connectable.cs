using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.Beta
{
    public class Connectable : MonoBehaviour
    {
        public enum HierarchalStructures {matriarchal, patriarchal};                // determines the type of hierarchy for this connectable
        public HierarchalStructures hierarchalStructure;                            // if matriarchal, males connectors come and go
        public List<Connector> connectors = new List<Connector>();                  // a list of connectors this object has
        public bool addMassToParent;                                                // when connected changes the mass of the parent rigidbody
        [HideInInspector]
        public float startMass = -1f;                                               // if the connectable has a rigidbody, stores the original mass of the object

        private void Start()
        {
            // if there is a ConnectableManager
            if (ConnectableManager.Instance)
            {
                ConnectableManager.Instance.connectables.Add(this);
            }

            if (GetComponent<Rigidbody>())
            {
                startMass = GetComponent<Rigidbody>().mass;
            }
        }

        private void OnDestroy()
        {
            // if there is a ConnectableManager
            if (ConnectableManager.Instance)
            {
                ConnectableManager.Instance.connectables.Remove(this);
            }
        }
    }
}

