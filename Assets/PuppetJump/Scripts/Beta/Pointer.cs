using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Objs;

namespace PuppetJump.Beta
{
    [RequireComponent(typeof(LineRenderer))]
    public class Pointer : MonoBehaviour
    {
        private Vector3[] linePoints = new Vector3[2];  // array of points for line to draw
        public int pointablesLayerNum = -1;             // if using a layer for pointables, set to layer number. (-1 will search all layers)
        [HideInInspector]
        public Pointable pointingAt;            // current pointed at object
        public float defaultLength = 0.5f;      // how far does the line renderer show for the pointer
        public GameObject reticle;              // a reticle for thew pointer
        public bool drawLine = true;            // an option to draw a line from the pointer to the teleport location while searching

        void Update()
        {
            Pointing();
        }

        void Pointing()
        {
            // Create a ray that points forwards from the pointer
            Ray ray = new Ray(transform.position + (transform.forward.normalized * 0.1f), transform.forward);
            RaycastHit hit;

            linePoints[0] = transform.position + (transform.forward.normalized * 0.1f);
            linePoints[1] = transform.position + (transform.forward.normalized * defaultLength);

            // if using a layer mask for pointables
            if (pointablesLayerNum != -1)
            {
                int lm = 1 << pointablesLayerNum;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, lm))
                {
                    CheckHitPointable(hit);
                }
                else
                {
                    // if nothing is being hit by the raycast
                    ClearPointedObject();
                }
            }
            else if (pointablesLayerNum == -1)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    CheckHitPointable(hit);
                }
                else
                {
                    // if nothing is being hit by the raycast
                    ClearPointedObject();
                }
            }

            if (drawLine)
            {
                DrawLine(linePoints);
            }

            if (reticle != null)
            {
                reticle.transform.position = linePoints[1];
            }
        }

        public void ActivatePointable()
        {
            if(pointingAt != null)
            {
                pointingAt.pointEvents.activate.Invoke();
            }
        }

        /// <summary>
        /// Checks if the object hit by the ray cast is a pointable.
        /// </summary>
        /// <param name="hit"></param>
        private void CheckHitPointable(RaycastHit hit)
        {
            // if the object hit by the raycast is a pointable object
            if (hit.collider.GetComponent<Pointable>() && hit.collider.GetComponent<Pointable>().isPointable)
            {
                // if there was no pointed at object
                if (pointingAt == null)
                {
                    pointingAt = hit.collider.GetComponent<Pointable>();
                    pointingAt.pointEvents.pointedAt.Invoke();
                }
                else if (pointingAt != null && pointingAt != hit.collider.GetComponent<Pointable>())
                {
                    // if there was a pointed at object, but this is a different one
                    pointingAt.pointEvents.unpointedAt.Invoke();
                    pointingAt = hit.collider.GetComponent<Pointable>();
                    pointingAt.pointEvents.pointedAt.Invoke();
                }

                // set the second line point to hit
                linePoints[1] = hit.point;

            // if no pointables are being hit by the pointer
            }
            else
            {
                ClearPointedObject();
            }
        }

        /// <summary>
        /// Clears the pointed at object.
        /// </summary>
        private void ClearPointedObject()
        {
            if (pointingAt != null)
            {
                pointingAt.pointEvents.unpointedAt.Invoke();
                pointingAt = null;
            }
        }

        /// <summary>
        /// Draws a line in the game that starts at the pointer and runs 
        /// along a series of points to the desired teleport location.
        /// </summary>
        /// <param name="linePoints">A list of points between the pointer and a teleport location.</param>
        void DrawLine(Vector3[] linePoints)
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            lr.enabled = true;
            lr.SetPositions(linePoints);
        }
    }
}
