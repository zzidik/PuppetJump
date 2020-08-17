using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuppetJump;
using PuppetJump.Utils;

namespace PuppetJump.Utils
{
    [RequireComponent(typeof(LineRenderer))]
    public class Teleporter : MonoBehaviour
    {
        public int teleportLayerNum;                                // the layer of teleport locations for raycast to hit, will ignore all others
        private CameraRig cameraRig;                                // the active camera rig
        public float maxTeleportDistance = 10.0f;                   // the maximum distance of a teleport
        public float minTeleportDistance = 3.0f;                    // the minimum distance of a teleport
        public enum PointerTypes { Straight, FallOff };             // optional methods of pointing, either in a straight line on in an arc
        public PointerTypes pointerType;                            // the method of pointing
        public Transform reticle;                                   // an optional object used to identify the location of a purposed teleport
        public bool drawLine = true;                                // an option to draw a line from the pointer to the teleport location while searching
        public bool instantTeleport = true;                         // an option to teleport instantly to a found location
        public float teleportSpeed = 1.0f;                          // the speed of a teleport if instant teleport is not selected
        public AnimationCurve teleportCurve;                        // the animation curve for the movement of the teleport if instant teleport is not selected
        private Vector3 initialPos;                                 // the start position of the camera rig before the teleport
        private Vector3 finalPos;                                   // the position of the camera rig after the teleport
        private bool onTeleportSpot = false;                        // true if during a search the ray cast is hitting a spot that can be teleported to
        private Vector3 teleportTo;                                 // the location that a teleport will move the camera rig to
        [HideInInspector]
        public Vector3 teleportLoc;                                 // the adjusted position of a teleport
        [HideInInspector]
        public GameObject teleportToGO_hit;                         // while searching for a place to teleport, the gameobejct of the hit
        [HideInInspector]
        public GameObject teleportToGO_last;                        // after a teleport takes place, stores the last game object of the teleport location
        private float rayLengthOut = 3.0f;                          // length of a ray cast to find a teleport location                                   
        private IEnumerator move;                                                   // holder of a coroutine to move during a teleport
        public enum TeleportStatus { Idle, Searching, StartTeleporting, Moving };   // possible states of the teleporter
        [HideInInspector]
        public TeleportStatus teleportStatus;                                       // current status of the teleporter
        [HideInInspector]
        public TeleportStatus teleportStatusCheck;                                  // once a new status has been delcared, this checks if all adjustments wher made for that status
        private Vector3 rigPos;
        private Quaternion rigRot;
        private PuppetRig.ViewTypes setView;


        public virtual void Start()
        {
            // initialize the move coroutine
            move = Move();
            // set the initial state of the teleporter
            teleportStatus = TeleportStatus.Idle;
            // hide the reticle
            ShowReticle(false);
        }

        public void StartSearchForTeleport()
        {
            cameraRig = PuppetJumpManager.Instance.cameraRig;
            teleportStatus = TeleportStatus.Searching;
        }

        public void EndSearchForTeleport()
        {
            if (teleportStatus == TeleportStatus.Searching)
            {
                teleportStatus = TeleportStatus.StartTeleporting;
            }
        }

        /// <summary>
        /// Draws a line in the game that starts at the pointer and runs 
        /// along a series of points to the desired teleport location.
        /// </summary>
        /// <param name="wayPoints">A list of points between the pointer and a teleport location.</param>
        void DrawLine(Vector3[] wayPoints)
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = true;

            Vector3[] points = MakeSmoothCurve(wayPoints, 10f);
            int numPoints = points.Length;

            lineRenderer.positionCount = numPoints;
            int vertexCount = 0;
            for (int j = 0; j < numPoints; j++)
            {
                lineRenderer.SetPosition(vertexCount, points[j]);
                vertexCount++;
            }
        }

        /// <summary>
        /// Stops the drawing of a line.
        /// </summary>
        void ClearLine()
        {
            if (drawLine)
            {
                if (GetComponent<LineRenderer>())
                {
                    GetComponent<LineRenderer>().enabled = false;
                }
            }
        }

        /// <summary>
        /// Used to determined the tilt up and down from a Quaternion rotation.
        /// </summary>
        /// <param name="qrot">The Quaternion rotation of the object.</param>
        /// <returns>The tilt up and down in degrees between -180 and 180.</returns>
        public static float GetPitch(Quaternion qrot)
        {
            // calculate pitch from quaternion rotation
            float pitch = Mathf.Atan2(2 * qrot.x * qrot.w - 2 * qrot.y * qrot.z, 1 - 2 * qrot.x * qrot.x - 2 * qrot.z * qrot.z);
            // change from radians to degrees between -180 and 180
            pitch *= 180 / Mathf.PI;
            return pitch;
        }

        /// <summary>
        /// Controls what happens during a search when a teleport location is found.
        /// </summary>
        /// <param name="wayPoints">A list of points between the pointer and a teleport location.</param>
        /// <param name="hit">A Raycast hit on a teleport location.</param>
        public void FoundTeleportLocation(Vector3[] wayPoints, RaycastHit hit)
        {
            onTeleportSpot = true;
            //vector3 location to teleport to
            teleportTo = hit.point;
            //game object of the telport location found
            teleportToGO_hit = hit.transform.gameObject;

            //if drawing a line for the teleport
            if (drawLine)
            {
                if (teleportStatus == TeleportStatus.Searching)
                {
                    DrawLine(wayPoints);
                }
                else
                {
                    ClearLine();
                }
            }

            //if using a reticle on the teleport to spot
            if (reticle)
            {

                //set the position of the reticle to the hit
                SetReticlePosition(hit);

                if (teleportStatus == TeleportStatus.Searching)
                {
                    //show the reticle
                    ShowReticle(true);
                }
                else
                {
                    //hide the reticle
                    ShowReticle(false);
                }
            }
        }

        /// <summary>
        /// Provides a way to stop the move coroutine from other scripts.
        /// </summary>
        public void KillMove()
        {
            StopCoroutine(move);
        }

        public void LostTeleportLocation()
        {
            //not on a spot that can be teleported to
            onTeleportSpot = false;

            ClearLine();

            ShowReticle(false);

            teleportToGO_hit = null;
        }

        /// <summary>
        /// Uses the pointer to search in a straight line for a teleport location.
        /// </summary>
        private void SearchForTeleportStraight()
        {
            // Create a ray that points forwards from the controller
            Ray ray = new Ray(transform.position + (transform.forward.normalized * 0.1f), transform.forward);
            RaycastHit hit;

            rayLengthOut = maxTeleportDistance;

            List<Vector3> wayPoints = new List<Vector3>();

            //first point is where the controller is
            wayPoints.Add(transform.position + (transform.forward.normalized * 0.1f));
            //second point is projected foward a distance from the controller
            wayPoints.Add(transform.position + (transform.forward.normalized * rayLengthOut));

            // Bit shift the index of the teleport layer to get a bit mask
            // This will cast rays only against colliders in teleportLayerNum.
            int layerMask = 1 << teleportLayerNum;

            // if instead we want to collide against everything except teleportLayerNum. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hit, rayLengthOut, layerMask))
            {
                //if hit a valid teleport location with in the rayLengthOut
                if (hit.collider.GetComponent<TeleportLocation>() && hit.collider.GetComponent<TeleportLocation>().active)
                {
                    wayPoints[1] = hit.point;
                    FoundTeleportLocation(wayPoints.ToArray(), hit);
                }
                //if not hit with in the ray length out
                else
                {
                    LostTeleportLocation();
                }
            }
            else
            {
                LostTeleportLocation();
            }
        }

        /// <summary>
        /// Uses the pointer to search in a arced line for a teleport location.
        /// </summary>
        void SearchForTeleportFallOff()
        {
            float pitchOfPointer = GetPitch(transform.rotation);
            //at -45 pitch, the arc should cast furthest beyond the rayLengthOut
            float percFallOff = 0.5f;

            //if controller is pointer up
            if (pitchOfPointer < 0.0f)
            {
                if (Mathf.Abs(pitchOfPointer) < 45.0f)
                {

                    float distMinMax = maxTeleportDistance - minTeleportDistance;
                    float perc = ((Mathf.Abs(pitchOfPointer) * 100) / 45) / 100;
                    rayLengthOut = minTeleportDistance + (distMinMax * perc);
                    percFallOff = 0.5f * perc;

                }
                else
                {

                    float distMinMax = maxTeleportDistance - minTeleportDistance;
                    float perc = (((90 - Mathf.Abs(pitchOfPointer)) * 100) / 45) / 100;
                    rayLengthOut = minTeleportDistance + (distMinMax * perc);
                    percFallOff = 0.5f * perc;
                }

             // if controller is pointed down
            }
            else
            {
                rayLengthOut = minTeleportDistance;
                percFallOff = 0.0f;
            }


            List<Vector3> wayPoints = new List<Vector3>();

            //first point is where the controller is
            wayPoints.Add(transform.position + (transform.forward.normalized * 0.1f));
            //second point is projected foward a distance from the controller
            wayPoints.Add(transform.position + (transform.forward.normalized * rayLengthOut));

            // Create a ray that points forwards from the controller
            Ray ray = new Ray(transform.position + (transform.forward.normalized * 0.1f), transform.forward);
            RaycastHit hit;

            // Bit shift the index of the teleport layer to get a bit mask
            // This will cast rays only against colliders in teleportLayerNum.
            int layerMask = 1 << teleportLayerNum;

            // if instead we want to collide against everything except teleportLayerNum. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hit, rayLengthOut, layerMask))
            {

                //if hit a valid teleport location with in the rayLengthOut
                if (hit.collider.GetComponent<TeleportLocation>() && hit.collider.GetComponent<TeleportLocation>().active)
                {

                    FoundTeleportLocation(wayPoints.ToArray(), hit);

                // if not hit with in the ray length out
                }
                else
                {

                    LostTeleportLocation();
                }
            }
            else
            {

                //create a ray that starts a certain distance from the direction the controller is pointing, included some falloff distance (depending on the pitch),and drop straight down from there
                ray = new Ray(transform.position + (transform.forward.normalized * rayLengthOut) + (transform.forward.normalized * (rayLengthOut * percFallOff)), Vector3.down);

                //raycast and check for hit
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {

                    //if hit a valid teleport location
                    if (hit.collider.GetComponent<TeleportLocation>() && hit.collider.GetComponent<TeleportLocation>().active)
                    {

                        //add a third point to the way points somewhere along the ground that can be teleported to
                        wayPoints.Add(hit.point);

                        FoundTeleportLocation(wayPoints.ToArray(), hit);

                    // if not hiting a valid teleport location
                    }
                    else
                    {

                        LostTeleportLocation();
                    }
                }
                else
                {
                    LostTeleportLocation();
                }
            }
        }

        /// <summary>
        /// Sets the position of the reticle when there is a hit by the raycast of the search.
        /// </summary>
        /// <param name="hit">The Raycast hit result of a search.</param>
        void SetReticlePosition(RaycastHit hit)
        {
            //put the reticle at the hit point of the raycast
            Vector3 reticlePos = hit.point;
            reticlePos.y += 0.01f;
            reticle.position = reticlePos;

            // rotates the reticle to lay along the normal of the hit of the raycast
            reticle.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

            // rotates the reticle to face the direction the head is looking
            Vector3 lookPos = PuppetJumpManager.Instance.puppetRig.GetComponent<PuppetRigVR>().puppetStrings.head.transform.position - reticle.position;
            lookPos.y = 0;
            reticle.rotation = Quaternion.LookRotation(lookPos);
        }

        /// <summary>
        /// Used to position, or begin the moving of, the puppetRig to the teleport location.
        /// </summary>
        public virtual void Teleport()
        {
            //move the camera rig to the position of the teleport
            teleportLoc = teleportTo;

            setView = PuppetJumpManager.Instance.puppetRig.views.view;
            float saveRot = cameraRig.cam.transform.eulerAngles.y;

            // store the positio and rotation of the cameraRig
            rigPos = cameraRig.transform.position;
            rigRot = cameraRig.transform.rotation;

            // the user is immediately placed in the teleport location
            if (setView != PuppetRig.ViewTypes.firstPerson)
            {
                // switch to first person
                PuppetJumpManager.Instance.puppetRig.SetView(PuppetRig.ViewTypes.firstPerson);
            }

            // adjust to allow for the offset of the camera inside the camera rig
            teleportLoc.x = teleportTo.x - (cameraRig.cam.transform.position.x - cameraRig.transform.position.x);
            teleportLoc.z = teleportTo.z - (cameraRig.cam.transform.position.z - cameraRig.transform.position.z);

            // since we switched to first person to determine the final position of the teleport
            if (setView != PuppetRig.ViewTypes.firstPerson)
            {
                // switch it back to the set view
                PuppetJumpManager.Instance.puppetRig.SetView(setView, rigPos, rigRot);
            }

            GameObject head = PuppetJumpManager.Instance.puppetRig.GetComponent<PuppetRigVR>().puppetStrings.head.gameObject;

            if (instantTeleport)
            {
                // move instantly to the new position
                PuppetJumpManager.Instance.puppetRig.transform.position = teleportLoc;

                // update the height and center charcater controller
                float newHeight = head.transform.localPosition.y;
                PuppetJumpManager.Instance.puppetRig.characterController.height = newHeight;
                Vector3 newCenter = PuppetJumpManager.Instance.puppetRig.characterController.center;
                newCenter = new Vector3(head.transform.localPosition.x, PuppetJumpManager.Instance.puppetRig.transform.position.y + (newHeight * 0.5f), head.transform.localPosition.z);
                PuppetJumpManager.Instance.puppetRig.characterController.center = newCenter;
            }
            else
            {
                // kill any currently running move coroutine
                StopCoroutine(move);

                // get the current position of the puppetRig
                initialPos = PuppetJumpManager.Instance.puppetRig.transform.position;
                // set the final position of the puppetRig after the teleport
                finalPos = teleportLoc;

                // set and start the new move coroutine
                move = Move();
                StartCoroutine(move);
            }

            //game object of the last telport
            teleportToGO_last = teleportToGO_hit;
        }

        /// <summary>
        /// A coroutine that moves the camera rig to the teleport location over time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Move()
        {
            if (teleportStatus == TeleportStatus.Moving)
            {
                yield break;
            }
            teleportStatus = TeleportStatus.Moving;

            GameObject head = PuppetJumpManager.Instance.puppetRig.GetComponent<PuppetRigVR>().puppetStrings.head.gameObject;

            float i = 0;
            float rate = 1 / teleportSpeed;
            while (i < 1)
            {
                teleportStatus = TeleportStatus.Moving;
                i += rate * Time.deltaTime;
                PuppetJumpManager.Instance.puppetRig.transform.position = Vector3.Lerp(initialPos, finalPos, teleportCurve.Evaluate(i));

                // update the height and center charcater controller
                float newHeight = head.transform.localPosition.y;
                PuppetJumpManager.Instance.puppetRig.characterController.height = newHeight;
                Vector3 newCenter = PuppetJumpManager.Instance.puppetRig.characterController.center;
                newCenter = new Vector3(head.transform.localPosition.x, PuppetJumpManager.Instance.puppetRig.transform.position.y + (newHeight * 0.5f), head.transform.localPosition.z);
                PuppetJumpManager.Instance.puppetRig.characterController.center = newCenter;

                yield return null;
            }

            teleportStatus = TeleportStatus.Idle;
        }

        /// <summary>
        /// Uses a controller to change the status of the teleporter.
        /// Then handles that status.
        /// </summary>
        void Update()
        {
            if (teleportStatus == TeleportStatus.Idle && teleportStatusCheck != TeleportStatus.Idle)
            {
                ClearLine();
                ShowReticle(false);
                teleportStatusCheck = TeleportStatus.Idle;
            }

            if (teleportStatus == TeleportStatus.Searching)
            {
                //continually check for teleport 
                if (pointerType == PointerTypes.Straight)
                {
                    SearchForTeleportStraight();
                }
                else if (pointerType == PointerTypes.FallOff)
                {
                    SearchForTeleportFallOff();
                }

                teleportStatusCheck = TeleportStatus.Searching;
            }

            if (teleportStatus == TeleportStatus.StartTeleporting && teleportStatusCheck != TeleportStatus.StartTeleporting)
            {
                //if on a spot that could be teleported to
                if (onTeleportSpot)
                {
                    Teleport();
                }

                teleportStatusCheck = TeleportStatus.StartTeleporting;

                if (instantTeleport)
                {
                    teleportStatus = TeleportStatus.Idle;
                }
            }

            if (teleportStatus == TeleportStatus.Moving && teleportStatusCheck != TeleportStatus.Moving)
            {
                ClearLine();
                ShowReticle(false);
                teleportStatusCheck = TeleportStatus.Moving;
            }
        }

        /// <summary>
        /// Controls the display of the reticle.
        /// </summary>
        /// <param name="r">True shows the reticle.</param>
        private void ShowReticle(bool r)
        {
            //when using a reticle
            if (reticle)
            {
                //hide the reticle
                //reticle.gameObject.GetComponent<Renderer>().enabled = r;
                reticle.gameObject.SetActive(r);
            }
        }

        /// <summary>
        /// Takes a list of Vector3s adds more depending on how smooth 
        /// the transition between points needs to be.
        /// </summary>
        /// <param name="arrayToCurve">The original Vector3 array.</param>
        /// <param name="smoothness">The number of interpolations.</param>
        /// <returns>An array of Vector3s.</returns>
        public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
        {
            List<Vector3> points;
            List<Vector3> curvedPoints;
            int pointsLength = 0;
            int curvedLength = 0;

            if (smoothness < 1.0f) smoothness = 1.0f;

            pointsLength = arrayToCurve.Length;

            curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
            curvedPoints = new List<Vector3>(curvedLength);

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                points = new List<Vector3>(arrayToCurve);

                for (int j = pointsLength - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        points[i] = (1 - t) * points[i] + t * points[i + 1];
                    }
                }

                curvedPoints.Add(points[0]);
            }
            return (curvedPoints.ToArray());
        }

        private Vector3 GetTeleportLoc()
        {
            teleportLoc = teleportTo;

            //adjust to allow for the offset of the camera inside the camera rig
            teleportLoc.x = teleportTo.x - (cameraRig.cam.transform.position.x - cameraRig.transform.position.x);
            teleportLoc.z = teleportTo.z - (cameraRig.cam.transform.position.z - cameraRig.transform.position.z);

            return teleportLoc;
        }
    }
}
