using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WayPointPath : MonoBehaviour
{
    public string ID;                                           // an ID for the path
    public List<WayPoint> waypoints = new List<WayPoint>();     // a list of waypoints that make up a path
    [System.Serializable]
    public class PathEvents
    {
        public UnityEvent endReached = new UnityEvent();        // trigger event(s) when a moving obejct reaches the last waypoint in the list
    }
    public PathEvents pathEvents;
}
