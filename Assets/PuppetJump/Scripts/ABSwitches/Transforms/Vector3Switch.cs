using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.ABSwitches
{
    /// <summary>
    /// An ABSwitch for changing an object's position, local position, euler angles, or local euler angles.
    /// </summary>
    public class Vector3Switch : ABSwitch
    {
        public enum Types {position,localPosition,eulerAngles,localEulerAngeles};
        public Types type = Types.position;                         // what type of Vector3 is the switch effecting
        private Vector3 AVector3;                                   // store the start local position as position A
        private Vector3 startVector3;                               // the vector3 when a switch starts
        private AnimationCurve activeCurve;                         // the active curve for the switch
        private Vector3 target;                                     // the target position of a switch (will be either BPosition or APosition)
        private float adjSpeed;                                     // the adjusted time of a switch (might be interupted by a new switch)

        public Vector3 BVector3;                                                        // store a new local position as posiiton B
        public float durationToB = 1.0f;                                                // time in seconds a switch to B takes
        public AnimationCurve switchToBCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);   // an animation curve for the switch to B
        public float durationToA = 1.0f;                                                // time in seconds a switch to A takes
        public AnimationCurve switchToACurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);   // an animation curve for the switch to A
        public bool loop = false;                                                       // sets the switch to loop between A and B, loop still needs to be started by a Switch() call


        private void Awake()
        {
            switch (type)
            {
                case Types.position:
                    AVector3 = transform.position;
                    break;
                case Types.localPosition:
                    AVector3 = transform.localPosition;
                    break;
                case Types.eulerAngles:
                    AVector3 = transform.eulerAngles;
                    break;
                case Types.localEulerAngeles:
                    AVector3 = transform.localEulerAngles;
                    break;
            }
            switchCoroutine = SwitchCoroutine();
            adjSpeed = durationToB;
        }

        /// <summary>
        /// If a switch is set to loop. It will loop when the object is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (loop)
            {
                Switch();
            }
        }

        /// <summary>
        /// Sets the parameters and starts the coroutine of a switch in motion.
        /// </summary>
        public override void Switch()
        {
            if (state == SwitchState.atA || state == SwitchState.switchingToA)
            {
                // change state to switching to B
                state = SwitchState.switchingToB;
                StopSwitch();

                target = BVector3;
                activeCurve = switchToBCurve;

                // because the switch might be interupted, adjust the duration of the switch base on how far it is from it's traget
                float distFromFinish = 0f;
                switch (type)
                {
                    case Types.position:
                        distFromFinish = Vector3.Distance(BVector3, transform.position);
                        startVector3 = transform.position;
                        break;
                    case Types.localPosition:
                        distFromFinish = Vector3.Distance(BVector3, transform.localPosition);
                        startVector3 = transform.localPosition;
                        break;
                    case Types.eulerAngles:
                        distFromFinish = Vector3.Distance(BVector3, transform.eulerAngles);
                        startVector3 = transform.eulerAngles;
                        break;
                    case Types.localEulerAngeles:
                        distFromFinish = Vector3.Distance(BVector3, transform.localEulerAngles);
                        startVector3 = transform.localEulerAngles;
                        break;
                }
                float toatlDistance = Vector3.Distance(AVector3, BVector3);
                float perc = ((distFromFinish * 100) / toatlDistance) / 100;
                adjSpeed = durationToB * perc;

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    switch (type)
                    {
                        case Types.position:
                            transform.position = target;
                            break;
                        case Types.localPosition:
                            transform.localPosition = target;
                            break;
                        case Types.eulerAngles:
                            transform.eulerAngles = target;
                            break;
                        case Types.localEulerAngeles:
                            transform.localEulerAngles = target;
                            break;
                    }
                    state = SwitchState.atB;
                    switchEvents.switchToBComplete.Invoke();
                    if (loop)
                    {
                        // if loop is not scheduled to end here
                        if (!endAtB)
                        {
                            Switch();
                        }
                        else
                        {
                            loop = false;
                            endAtB = false;
                        }
                    }
                }
                else
                {
                    StartSwitch();
                }
            }
            else if (state == SwitchState.atB || state == SwitchState.switchingToB)
            {
                // change state to switching to A
                state = SwitchState.switchingToA;
                StopSwitch();

                target = AVector3;
                activeCurve = switchToACurve;

                // because the animation might be interupted, need to adjust the time of the animation base on how far it is from it's traget
                float distFromStart = 0f;
                switch (type)
                {
                    case Types.position:
                        distFromStart = Vector3.Distance(AVector3, transform.position);
                        startVector3 = transform.position;
                        break;
                    case Types.localPosition:
                        distFromStart = Vector3.Distance(AVector3, transform.localPosition);
                        startVector3 = transform.localPosition;
                        break;
                    case Types.eulerAngles:
                        distFromStart = Vector3.Distance(AVector3, transform.eulerAngles);
                        startVector3 = transform.eulerAngles;
                        break;
                    case Types.localEulerAngeles:
                        distFromStart = Vector3.Distance(AVector3, transform.localEulerAngles);
                        startVector3 = transform.localEulerAngles;
                        break;
                }
                float toatlDistance = Vector3.Distance(AVector3, BVector3);
                float perc = ((distFromStart * 100) / toatlDistance) / 100;
                adjSpeed = durationToA * perc;

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    switch (type)
                    {
                        case Types.position:
                            transform.position = target;
                            break;
                        case Types.localPosition:
                            transform.localPosition = target;
                            break;
                        case Types.eulerAngles:
                            transform.eulerAngles = target;
                            break;
                        case Types.localEulerAngeles:
                            transform.localEulerAngles = target;
                            break;
                    }
                    state = SwitchState.atA;
                    switchEvents.switchToAComplete.Invoke();
                    if (loop)
                    {
                        // if loop is not scheduled to end here
                        if (!endAtA)
                        {
                            Switch();
                        }
                        else
                        {
                            loop = false;
                            endAtA = false;
                        }
                    }
                }
                else
                {
                    StartSwitch();
                }
            }
        }

        /// <summary>
        /// Sets the parameters and starts the coroutine of a switch in motion
        /// towards a particular state.
        /// </summary>
        public void SwitchTo(SwitchState toState)
        {
            if (toState == SwitchState.switchingToB || toState == SwitchState.atB || toState == SwitchState.toB)
            {
                // change state to switching to B
                state = SwitchState.switchingToB;
                StopSwitch();

                target = BVector3;
                activeCurve = switchToBCurve;

                // because the switch might be interupted, adjust the duration of the switch base on how far it is from it's traget
                float distFromFinish = 0f;
                switch (type)
                {
                    case Types.position:
                        distFromFinish = Vector3.Distance(BVector3, transform.position);
                        startVector3 = transform.position;
                        break;
                    case Types.localPosition:
                        distFromFinish = Vector3.Distance(BVector3, transform.localPosition);
                        startVector3 = transform.localPosition;
                        break;
                    case Types.eulerAngles:
                        distFromFinish = Vector3.Distance(BVector3, transform.eulerAngles);
                        startVector3 = transform.eulerAngles;
                        break;
                    case Types.localEulerAngeles:
                        distFromFinish = Vector3.Distance(BVector3, transform.localEulerAngles);
                        startVector3 = transform.localEulerAngles;
                        break;
                }
                float toatlDistance = Vector3.Distance(AVector3, BVector3);
                float perc = ((distFromFinish * 100) / toatlDistance) / 100;
                adjSpeed = durationToB * perc;

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    switch (type)
                    {
                        case Types.position:
                            transform.position = target;
                            break;
                        case Types.localPosition:
                            transform.localPosition = target;
                            break;
                        case Types.eulerAngles:
                            transform.eulerAngles = target;
                            break;
                        case Types.localEulerAngeles:
                            transform.localEulerAngles = target;
                            break;
                    }
                    state = SwitchState.atB;
                    switchEvents.switchToBComplete.Invoke();
                    if (loop)
                    {
                        // if loop is not scheduled to end here
                        if (!endAtB)
                        {
                            Switch();
                        }
                        else
                        {
                            loop = false;
                            endAtB = false;
                        }
                    }
                }
                else
                {
                    StartSwitch();
                }
            }
            else if (toState == SwitchState.switchingToA || toState == SwitchState.atA || toState == SwitchState.toA)
            {
                // change state to switching to A
                state = SwitchState.switchingToA;
                StopSwitch();

                target = AVector3;
                activeCurve = switchToACurve;

                // because the animation might be interupted, need to adjust the time of the animation base on how far it is from it's traget
                float distFromStart = 0f;
                switch (type)
                {
                    case Types.position:
                        distFromStart = Vector3.Distance(AVector3, transform.position);
                        startVector3 = transform.position;
                        break;
                    case Types.localPosition:
                        distFromStart = Vector3.Distance(AVector3, transform.localPosition);
                        startVector3 = transform.localPosition;
                        break;
                    case Types.eulerAngles:
                        distFromStart = Vector3.Distance(AVector3, transform.eulerAngles);
                        startVector3 = transform.eulerAngles;
                        break;
                    case Types.localEulerAngeles:
                        distFromStart = Vector3.Distance(AVector3, transform.localEulerAngles);
                        startVector3 = transform.localEulerAngles;
                        break;
                }
                float toatlDistance = Vector3.Distance(AVector3, BVector3);
                float perc = ((distFromStart * 100) / toatlDistance) / 100;
                adjSpeed = durationToA * perc;

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    switch (type)
                    {
                        case Types.position:
                            transform.position = target;
                            break;
                        case Types.localPosition:
                            transform.localPosition = target;
                            break;
                        case Types.eulerAngles:
                            transform.eulerAngles = target;
                            break;
                        case Types.localEulerAngeles:
                            transform.localEulerAngles = target;
                            break;
                    }
                    state = SwitchState.atA;
                    switchEvents.switchToAComplete.Invoke();
                    if (loop)
                    {
                        // if loop is not scheduled to end here
                        if (!endAtA)
                        {
                            Switch();
                        }
                        else
                        {
                            loop = false;
                            endAtA = false;
                        }
                    }
                }
                else
                {
                    StartSwitch();
                }
            }
        }

        /// <summary>
        /// Starts a new coroutine for the switch.
        /// </summary>
        private void StartSwitch()
        {
            StartCoroutine(switchCoroutine);
        }

        /// <summary>
        /// Stops the current coroutine of the switch and resets it.
        /// </summary>
        private void StopSwitch()
        {
            StopCoroutine(switchCoroutine);
            switchCoroutine = null;
            switchCoroutine = SwitchCoroutine();
        }

        /// <summary>
        /// The coroutine of the switch.
        /// </summary>
        /// <returns>Null</returns>
        private IEnumerator SwitchCoroutine()
        {
            float i = 0;
            float rate = 1 / adjSpeed;

            while (i < 1)
            {
                i += rate * Time.deltaTime;
                switch (type)
                {
                    case Types.position:
                        transform.position = Vector3.Lerp(startVector3, target, activeCurve.Evaluate(i));
                        break;
                    case Types.localPosition:
                        transform.localPosition = Vector3.Lerp(startVector3, target, activeCurve.Evaluate(i));
                        break;
                    case Types.eulerAngles:
                        Quaternion qTo = Quaternion.Euler(target);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, qTo, activeCurve.Evaluate(i));
                        break;
                    case Types.localEulerAngeles:
                        Quaternion qLTo = Quaternion.Euler(target);
                        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, qLTo, activeCurve.Evaluate(i));
                        break;
                }
                yield return null;
            }

            // when a switch is completed
            // trigger events based on which switch just finished
            if (state == SwitchState.switchingToA)
            {
                // make sure the switch finished at its destination
                SetToA();
                switchEvents.switchToAComplete.Invoke();
                if (loop)
                {
                    // if loop is not scheduled to end here
                    if (!endAtA)
                    {
                        Switch();
                    }
                    else
                    {
                        loop = false;
                        endAtA = false;
                    }
                }
            }
            else if (state == SwitchState.switchingToB)

            {
                // make sure the switch finished at its destination
                SetToB();
                switchEvents.switchToBComplete.Invoke();
                if (loop)
                {
                    // if loop is not scheduled to end here
                    if (!endAtB)
                    {
                        Switch();
                    }
                    else
                    {
                        loop = false;
                        endAtB = false;
                    }
                }
            }
        }

        /// <summary>
        /// In some cases it may be necessary to reset the AVector3 after start.
        /// </summary>
        /// <param name="newPos">The new position.</param>
        public void ResetA(Vector3 newPos)
        {
            AVector3 = newPos;
        }

        /// <summary>
        /// Sets the switch to position A.
        /// No invoke of events.
        /// </summary>
        public override void SetToA()
        {
            switch (type)
            {
                case Types.position:
                    transform.position = AVector3;
                    break;
                case Types.localPosition:
                    transform.localPosition = AVector3;
                    break;
                case Types.eulerAngles:
                    Quaternion qTo = Quaternion.Euler(AVector3);
                    transform.rotation = qTo;
                    break;
                case Types.localEulerAngeles:
                    Quaternion qLTo = Quaternion.Euler(AVector3);
                    transform.localRotation = qLTo;
                    break;
            }

            state = SwitchState.atA;
        }

        /// <summary>
        /// Sets the switch to position B.
        /// No invoke of events.
        /// </summary>
        public override void SetToB()
        {
            switch (type)
            {
                case Types.position:
                    transform.position = BVector3;
                    break;
                case Types.localPosition:
                    transform.localPosition = BVector3;
                    break;
                case Types.eulerAngles:
                    Quaternion qTo = Quaternion.Euler(BVector3);
                    transform.rotation = qTo;
                    break;
                case Types.localEulerAngeles:
                    Quaternion qLTo = Quaternion.Euler(BVector3);
                    transform.localRotation = qLTo;
                    break;
            }

            state = SwitchState.atB;
        }
    }
}