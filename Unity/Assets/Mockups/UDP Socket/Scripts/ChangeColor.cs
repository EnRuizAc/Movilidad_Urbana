using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Material theMaterial;
    public Color green; // TypeColor = 5
    public Color red; // TypeColor = 6

    void SetColor(int i)
    {
        if (i == 5)
        {
            theMaterial.SetColor("_BaseColor", green);
        }
        theMaterial.SetColor("_BaseColor", red);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
