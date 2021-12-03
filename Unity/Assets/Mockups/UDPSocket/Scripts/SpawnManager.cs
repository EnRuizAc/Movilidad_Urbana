using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject carPrefab;
    // diccionario objs = {
    //  car_id: game object
    // }

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        GameObject car = Instantiate(carPrefab);
        car.transform.position = new Vector3(10, 10, 10);
    }

    public void UpdateAgents(InfoAgents agents)
    {
        // for agents
            // car_obj = objs[agents[i].id]
            // car.obj 
    }
}
