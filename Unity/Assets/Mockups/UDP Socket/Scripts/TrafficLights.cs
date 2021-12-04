using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TrafficLights : MonoBehaviour
{
    public int id;
    public Material theMaterial;
    public Color green; // TypeColor = 5
    public Color red; // TypeColor = 6

    public void SetColor(int i)
    {
        if (i == 5)
        {
            theMaterial.SetColor("_BaseColor", green);
        }
        else
        {
            theMaterial.SetColor("_BaseColor", red);
        }
        
    }

}
