using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuppetJump;
using PuppetJump.Objs;

public class KeyboardButton : PuppetJump.Objs.Button
{
    public string value;
    public Text valueDisplay;
    public Image backgroundImage;

    public void DisplayValue()
    {
        valueDisplay.text = value;
    }
}
