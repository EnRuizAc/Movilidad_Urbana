using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject carPrefab;

    public TrafficLights[] lights;


    InfoStatus _agents;
    List<Car> sceneCars = new List<Car>();
    public float dist_multiplier = 2;
    public float wait = 1;

    public void UpdateAgents(InfoStatus agents)
    {

        // Esto se ejecutaria una s�la vez.
        if(_agents == null)
        {
            for (int i = 0; i < agents.Cars.Count; i++)
            {
                Car car = Instantiate(carPrefab).GetComponent<Car>();
                car.SetTargetRotation(agents.Cars[i].Direction);
                car.transform.position = new Vector3(agents.Cars[i].Position.x * dist_multiplier, 0, agents.Cars[i].Position.z * dist_multiplier);
                car.id = agents.Cars[i].CarId;
                sceneCars.Add(car);
            }
        }

        _agents = agents;

        // Ejemplo de buscar por id.
        for (int i = 0; i < agents.Cars.Count; i++)
        {
            // Poner atenci�n en la interpretaci�n del orden de las coordenadas de python (X,Y) y de Unity (X,Y,Z),
            // porque la profundidad en Python es en el eje Y y en Unity ser�a Z � a sus criterios.
            Vector3 newPosition = new Vector3(agents.Cars[i].Position.x * dist_multiplier, 0, agents.Cars[i].Position.z * dist_multiplier);

            // Para buscar por id.
            Car car = sceneCars.Find(s => s.id == agents.Cars[i].CarId);
            car.SetTargetRotation(agents.Cars[i].Direction);
            car.SetTargetPosition(newPosition);
        }

        
    }

    public void RunModel(InfoSteps modelSteps)
    {
        StartCoroutine(Animate(modelSteps));
    }

    IEnumerator Animate(InfoSteps modelSteps)
    {
        InfoStatus currentStatus;
        for (int i = 0; i < modelSteps.steps.Count; i++)
        {
            currentStatus = modelSteps.steps[i];
            UpdateLights(currentStatus);
            UpdateAgents(currentStatus);
            yield return new WaitForSeconds(wait);
        }
    }

    private void UpdateLights(InfoStatus infoStatus)
    {
        for (int i = 0; i < infoStatus.TrafficLights.Count; i++)
        {
            InfoTrafficLights light = infoStatus.TrafficLights[i];
            lights[i].SetColor(light.TypeColor);
        }
    }
}
