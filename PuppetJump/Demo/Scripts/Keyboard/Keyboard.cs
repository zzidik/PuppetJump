using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuppetJump;
using PuppetJump.Utils;
using PuppetJump.Objs;
using PuppetJump.ABSwitches;

public class Keyboard : CanvasButtonGroup
{
    [System.Serializable]
    public class KeyValues
    {
        public string value;
        public string shiftValue;
    }
    public List<KeyValues> keyValues = new List<KeyValues>();
    public Text inputDisplay;
    [HideInInspector]
    public List<string> inputs = new List<string>();

    public override void SetUpGroup()
    {
        base.SetUpGroup();

        int numButtons = buttons.Count;
        for(int b = 0; b < numButtons; b++)
        {
            // asign value to key
            buttons[b].GetComponent<KeyboardButton>().value = keyValues[b].value;
            // display the value
            buttons[b].GetComponent<KeyboardButton>().DisplayValue();

            // add KeyPressed listener to key
            GameObject tempButton = buttons[b].gameObject;
            if (keyValues[b].value != "SHIFT")
            {
                buttons[b].GetComponent<KeyboardButton>().touchEvents.touch.AddListener(() => { KeyPressed(tempButton); });
            }

            // if value is SHIFT add shift listener
            if(keyValues[b].value == "SHIFT")
            {
                buttons[b].GetComponent<KeyboardButton>().isRadioButton = true;
                buttons[b].GetComponent<KeyboardButton>().touchEvents.touch.AddListener(() => { ShiftPressed(tempButton); });
            }

            // reset the AVector3 because this position was dynamically generated
            buttons[b].GetComponent<KeyboardButton>().backgroundImage.GetComponent<Vector3Switch>().ResetA(buttons[b].GetComponent<KeyboardButton>().backgroundImage.transform.localPosition);
        }
    }

    public void KeyPressed(GameObject button)
    {
        switch (button.GetComponent<KeyboardButton>().value)
        {
            case "BACK":
                // remove the last input
                int numInputs = inputs.Count;
                if(numInputs > 0)
                {
                    inputs.RemoveAt(numInputs - 1);
                }
                break;
            default:
                inputs.Add(button.GetComponent<KeyboardButton>().value);
                break;
        }

        // update the input display
        string inputString = string.Join("", inputs.ToArray());
        inputDisplay.text = inputString;
    }

    public void ShiftPressed(GameObject button)
    {
        // get each child of the group
        List<GameObject> buttons = PuppetJumpManager.Instance.GetChildren(this.gameObject);
        int numButtons = buttons.Count;
        for (int b = 0; b < numButtons; b++)
        {
            // if the shift button is down
            if (button.GetComponent<KeyboardButton>().isRadioButtonDown)
            {
                // asign value to key
                buttons[b].GetComponent<KeyboardButton>().value = keyValues[b].shiftValue;
            }
            else
            {
                // asign value to key
                buttons[b].GetComponent<KeyboardButton>().value = keyValues[b].value;
            }

            // display the value
            buttons[b].GetComponent<KeyboardButton>().DisplayValue();
        }
    }
}
