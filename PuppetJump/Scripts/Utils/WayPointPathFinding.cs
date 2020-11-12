using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PuppetJump.Utils
{
    public class WayPointPathFinding : MonoBehaviour
    {
        private int activePoint;                                            // the waypoint currently being targeted
        private IEnumerator moveCoroutine;                                  // a coroutine that controls the move
        private Vector3 startPos;                                           // the start position of a move
        private Vector3 endPos;                                             // the end position of a move
        private Quaternion startRot;                                        // the start rotation of rotate
        private Quaternion endRot;                                          // the end rotation of a rotate

        public List<WayPointPath> paths = new List<WayPointPath>();         // a list of paths available to this pathfinder
        [HideInInspector]
        public WayPointPath activePath;                                     // the path currently being used
        public float moveSpeed = 5f;                                        // the speed of movement from waypoint to waypoint
        public float isoRotationSpeed = 90f;                                // the speed of rotation when there is no movement require between waypoints
        

        private void Start()
        {
            moveCoroutine = MoveCoroutine();
        }

        public void Move()
        {
            StopMove();
            StartMove();
        }

        private void StartMove()
        {
            StartCoroutine(moveCoroutine);
        }

        /// <summary>
        /// Stops the current coroutine of the switch and resets it.
        /// </summary>
        private void StopMove()
        {
            if(moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            
            moveCoroutine = null;
            moveCoroutine = MoveCoroutine();
        }

        /// <summary>
        /// The coroutine that performs the move on the path finder.
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveCoroutine()
        {
            // find the distance to travel
            var dist = Vector3.Distance(startPos, endPos);
            // calculate the movement duration
            var duration = dist / moveSpeed;

            // if distance to travel is zero
            if (dist < 0.1f)
            {
                // there still might be rotation to happen
                dist = Quaternion.Angle(startRot, endRot);
                duration = dist / isoRotationSpeed;
            }

            // t is the control variable
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(startPos, endPos, activePath.waypoints[activePoint].curveIn.Evaluate(t));
                transform.rotation = Quaternion.Lerp(startRot, endRot, activePath.waypoints[activePoint].curveIn.Evaluate(t));
                yield return null;
            }

            // movement finished
            NextWayPoint();
        }

        /// <summary>
        /// Checks if there are mmore waypoints in the active path.
        /// If there are, sends the pathfinder to the next point.
        /// If there are no more points in the path, calls endReached event(s).
        /// </summary>
        private void NextWayPoint()
        {
            activePath.waypoints[activePoint].pointEvents.pointReached.Invoke();

            // are there more points on the path
            if (activePoint + 1 < activePath.waypoints.Count)
            {
                // set new start position and rotation
                startPos = transform.position;
                startRot = transform.rotation;

                // advance to next point
                activePoint += 1;
                endPos = activePath.waypoints[activePoint].transform.position;
                endRot = activePath.waypoints[activePoint].transform.rotation;
                Move();
            }
            // end of path
            else
            {
                activePath.pathEvents.endReached.Invoke();
            }
        }

        /// <summary>
        /// Gets a path by ID.
        /// </summary>
        public void LoadPathByID(string ID)
        {
            int numPaths = paths.Count;
            for (int p = 0; p < numPaths; p++)
            {
                if (paths[p].ID == ID)
                {
                    activePath = paths[p];
                    activePoint = 0;
                    startPos = transform.position;
                    startRot = transform.rotation;
                    endPos = activePath.waypoints[activePoint].transform.position;
                    endRot = activePath.waypoints[activePoint].transform.rotation;
                    //Debug.Log(ID + " path loaded.");
                    Move();
                    break;
                }
            }
        }

        public void ChangeSpeed(float s)
        {
            moveSpeed = s;
        }
    }
}
