using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{ 

    iPositionProvider provider;
    public Transform target;

    public float repeatingTime = 1;
    public float timeMultiplier = 1;
    public float roadMultiplier = 5;

    public CalculatePosition calculatePosition;

    // Start is called before the first frame update
    void Start()
    {
        provider = new PositionMockUp();
        InvokeRepeating("ReadPosition", 1, repeatingTime);
    }

    void ReadPosition()
    {
        Vector3 position = provider.GetPosition(Time.time * timeMultiplier) * roadMultiplier;
        //target.position = position;

        calculatePosition.SetPosition(position);
    }
}

public interface iPositionProvider
{
    Vector3 GetPosition(float Time);
}

public class PositionMockUp : iPositionProvider
{
    Vector3[] positions = new Vector3[] {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(2, 0, 0),
        new Vector3(3, 0, 0),
        new Vector3(4, 0, 0),
        new Vector3(4, 0, 1),
        new Vector3(4, 0, 2),
        new Vector3(4, 0, 3),
        new Vector3(4, 0, 4)
    };

    public Vector3 GetPosition(float time)
    {
        int index = (int)Mathf.PingPong(time, positions.Length);
        return positions[index];
    }
}