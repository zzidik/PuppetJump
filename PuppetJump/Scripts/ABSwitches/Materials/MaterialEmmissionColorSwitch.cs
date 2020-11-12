using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

namespace PuppetJump.ABSwitches
{
    /// <summary>
    /// An ABSwitch for changing an object's emission color of a material.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class MaterialEmmissionColorSwitch : ABSwitch
    {
        private Color AColor;                                                           // store the start color
        private Color startColor;                                                       // the color when a switch starts
        private float intensity;                                                        // the current intensity of the switch
        private float startIntensity;                                                   // the start value of intesity at a switch
        private AnimationCurve activeCurve;                                             // the active curve for the switch
        private Color target;                                                           // the target color of a switch (will be either BColor or AColor)
        private float targetIntensity;                                                  // the target inensity of a switch
        private float adjSpeed;                                                         // the adjusted time of a switch (might be interupted by a new switch)
        private GradientColorKey[] colorkeys = new GradientColorKey[2];                 // color keys in the gradient, one for orginal color and one for change to color
        private GradientAlphaKey[] alphakeys = new GradientAlphaKey[2];                 // alpha keys in the gradient, one for orginal color and one for change to color
        private Gradient gradient = new Gradient();                                     // a grdient used to slide between colors

        public float AIntensity;                                                        // the start intensity of the emission
        public Color BColor;                                                            // store a new color as posiiton B
        public float BIntensity;                                                        // store a new intensity
        public float durationToB = 1.0f;                                                // time in seconds a switch to B takes
        public AnimationCurve switchToBCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);   // an animation curve for the switch to B
        public float durationToA = 1.0f;                                                // time in seconds a switch to A takes
        public AnimationCurve switchToACurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);   // an animation curve for the switch to A
        public bool loop = false;                                                       // sets the switch to loop between A and B, loop still needs to be started by a Switch() call



        private void Awake()
        {
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            AColor = GetComponent<Renderer>().material.GetColor("_EmissionColor");
            intensity = AIntensity;
            switchCoroutine = SwitchCoroutine();
            adjSpeed = durationToB;

            colorkeys[0].color = AColor;
            colorkeys[0].time = 0f;
            colorkeys[1].color = BColor;
            colorkeys[1].time = 1f;

            alphakeys[0].alpha = AColor.a;
            alphakeys[0].time = 0f;
            alphakeys[1].alpha = BColor.a;
            alphakeys[1].time = 1f;

            gradient.SetKeys(colorkeys, alphakeys);
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
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

            if (state == SwitchState.atA || state == SwitchState.switchingToA)
            {
                // change state to switching to B
                state = SwitchState.switchingToB;
                StopSwitch();

                target = BColor;
                startIntensity = intensity;
                targetIntensity = BIntensity;
                activeCurve = switchToBCurve;

                // because the switch might be interupted, adjust the duration of the switch base on how far it is from it's traget
                float distFromFinish = Vector4.Distance(BColor, GetComponent<Renderer>().material.GetColor("_EmissionColor"));
                float toatlDistance = Vector4.Distance(AColor, BColor);
                float perc = ((distFromFinish * 100) / toatlDistance) / 100;
                adjSpeed = durationToB * perc;

                // set the keys for the gradient
                colorkeys[0].color = GetComponent<Renderer>().material.GetColor("_EmissionColor");
                colorkeys[0].time = 0f;
                colorkeys[1].color = BColor;
                colorkeys[1].time = 1f;

                alphakeys[0].alpha = GetComponent<Renderer>().material.color.a;
                alphakeys[0].time = 0f;
                alphakeys[1].alpha = BColor.a;
                alphakeys[1].time = 1f;

                gradient.SetKeys(colorkeys, alphakeys);

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    intensity = BIntensity;
                    GetComponent<Renderer>().material.SetColor("_EmissionColor", BColor * intensity);
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

                target = AColor;
                startIntensity = intensity;
                targetIntensity = AIntensity;
                activeCurve = switchToACurve;

                // because the animation might be interupted, need to adjust the time of the animation base on how far it is from it's traget
                float distFromStart = Vector4.Distance(AColor, GetComponent<Renderer>().material.GetColor("_EmissionColor"));
                float toatlDistance = Vector4.Distance(AColor, BColor);
                float perc = ((distFromStart * 100) / toatlDistance) / 100;
                adjSpeed = durationToA * perc;

                // set the keys for the gradient
                colorkeys[0].color = GetComponent<Renderer>().material.GetColor("_EmissionColor");
                colorkeys[0].time = 0f;
                colorkeys[1].color = AColor;
                colorkeys[1].time = 1f;

                alphakeys[0].alpha = GetComponent<Renderer>().material.color.a;
                alphakeys[0].time = 0f;
                alphakeys[1].alpha = AColor.a;
                alphakeys[1].time = 1f;

                gradient.SetKeys(colorkeys, alphakeys);

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    intensity = AIntensity;
                    GetComponent<Renderer>().material.SetColor("_EmissionColor", AColor * intensity);
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
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

            if (toState == SwitchState.switchingToB || toState == SwitchState.atB || toState == SwitchState.toB)
            {
                // change state to switching to B
                state = SwitchState.switchingToB;
                StopSwitch();

                target = BColor;
                startIntensity = intensity;
                targetIntensity = BIntensity;
                activeCurve = switchToBCurve;

                // because the switch might be interupted, adjust the duration of the switch base on how far it is from it's traget
                float distFromFinish = Vector4.Distance(BColor, GetComponent<Renderer>().material.GetColor("_EmissionColor"));
                float toatlDistance = Vector4.Distance(AColor, BColor);
                float perc = ((distFromFinish * 100) / toatlDistance) / 100;
                adjSpeed = durationToB * perc;

                // set the keys for the gradient
                colorkeys[0].color = GetComponent<Renderer>().material.GetColor("_EmissionColor");
                colorkeys[0].time = 0f;
                colorkeys[1].color = BColor;
                colorkeys[1].time = 1f;

                alphakeys[0].alpha = GetComponent<Renderer>().material.color.a;
                alphakeys[0].time = 0f;
                alphakeys[1].alpha = BColor.a;
                alphakeys[1].time = 1f;

                gradient.SetKeys(colorkeys, alphakeys);

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    intensity = BIntensity;
                    GetComponent<Renderer>().material.SetColor("_EmissionColor", BColor * intensity);
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

                target = AColor;
                startIntensity = intensity;
                targetIntensity = AIntensity;
                activeCurve = switchToACurve;

                // because the animation might be interupted, need to adjust the time of the animation base on how far it is from it's traget
                float distFromStart = Vector4.Distance(AColor, GetComponent<Renderer>().material.GetColor("_EmissionColor"));
                float toatlDistance = Vector4.Distance(AColor, BColor);
                float perc = ((distFromStart * 100) / toatlDistance) / 100;
                adjSpeed = durationToA * perc;

                // set the keys for the gradient
                colorkeys[0].color = GetComponent<Renderer>().material.GetColor("_EmissionColor");
                colorkeys[0].time = 0f;
                colorkeys[1].color = AColor;
                colorkeys[1].time = 1f;

                alphakeys[0].alpha = GetComponent<Renderer>().material.color.a;
                alphakeys[0].time = 0f;
                alphakeys[1].alpha = AColor.a;
                alphakeys[1].time = 1f;

                gradient.SetKeys(colorkeys, alphakeys);

                // if the duration of the switch is 0
                if (adjSpeed == 0)
                {
                    // do the switch without a coroutine
                    intensity = AIntensity;
                    GetComponent<Renderer>().material.SetColor("_EmissionColor", AColor * intensity);
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

            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

            while (i < 1)
            {
                i += rate * Time.deltaTime;
                float value = activeCurve.Evaluate(i);
                Color color = gradient.Evaluate(value);
                intensity = Mathf.Lerp(startIntensity, targetIntensity, activeCurve.Evaluate(i));
                GetComponent<Renderer>().material.SetColor("_EmissionColor", color * intensity);
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
        /// In some cases it may be necessary to reset the AColor after start.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        public void ResetA(Color newColor)
        {
            AColor = newColor;
        }

        /// <summary>
        /// Sets the switch to color A.
        /// No invoke of events.
        /// </summary>
        public override void SetToA()
        {
            intensity = AIntensity;
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            GetComponent<Renderer>().material.SetColor("_EmissionColor", AColor * intensity);
            state = SwitchState.atA;
        }

        /// <summary>
        /// Sets the switch to color B.
        /// No invoke of events.
        /// </summary>
        public override void SetToB()
        {
            intensity = BIntensity;
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            GetComponent<Renderer>().material.SetColor("_EmissionColor", BColor * intensity);
            state = SwitchState.atB;
        }
    }
}
