using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WayPoint : MonoBehaviour
{
    public AnimationCurve curveIn = AnimationCurve.Linear(0f, 0f, 1f, 1f);   // an animation curve for easing into a waypoint
    [System.Serializable]
    public class PointEvents
    {
        public UnityEvent pointReached = new UnityEvent();      // trigger event(s) when a moving obejct reaches this waypoint
    }
    public PointEvents pointEvents;
}
