using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuppetJump.Utils;

public class robot4Demo : MonoBehaviour
{
    void Start()
    {
        DoPath();
    }

    public void DoPath()
    {
        GetComponent<WayPointPathFinding>().LoadPathByID("figureEight");
    }
}
