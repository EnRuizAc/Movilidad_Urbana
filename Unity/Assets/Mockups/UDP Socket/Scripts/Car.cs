using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public int id;
    Vector3 targetPosition;
    Vector3 targetRotation;
    public float speed;
    public float speedRotation;

    void Start()
    {
        speed = 1;
        speedRotation = 5;
    }

    void Update()
    {
        // Con los ejercicios anteriores, buscar la manera de mejorar el smooth de posici�n y giro.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * speedRotation);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
        
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public void SetTargetRotation(string direction)
    {
        //Debug.Log(direction);
        if (direction == "right")
            targetRotation = new Vector3(0, 0, 0);
        else if (direction == "left")
        {
            targetRotation = new Vector3(0, 180, 0);
        }
        else if (direction == "down")
        {
            targetRotation = new Vector3(0, 90, 0);
        }
        else if (direction == "up")
        {
            targetRotation = new Vector3(0, -90, 0);
        }
    }
}
