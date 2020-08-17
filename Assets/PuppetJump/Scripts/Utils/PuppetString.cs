using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Objs;

namespace PuppetJump.Utils
{
    [RequireComponent(typeof(Rigidbody))]
    public class PuppetString : MonoBehaviour
    {
        [HideInInspector]
        public Rigidbody rigidbody;

        protected virtual void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
    }
}
