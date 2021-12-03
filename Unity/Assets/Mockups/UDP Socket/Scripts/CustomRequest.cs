using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomRequest : MonoBehaviour
{
    public string url = "https://multiagenteurbano-empathic-waterbuck.mybluemix.net/";
    //public Text statusText;
    //public Text titleResponse;

    private void Start()
    {
        Debug.Log("Starting request");
        //statusText.text = "Do Request";
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {// Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            //yield return new WaitForSeconds(2.5f);

            //Clean();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.Success:
                    //byte[] data = webRequest.downloadHandler.data;
                    //string jsonString = Encoding.UTF8.GetString(data);
                    
                    string jsonString = webRequest.downloadHandler.text;
                    Debug.Log(":\nReceived: " + jsonString);

                    InfoSteps modelSteps = JsonUtility.FromJson<InfoSteps>(jsonString);
                    //titleResponse.text = myObject.title;
                    Debug.Log(modelSteps.steps.Count);

                    break;
            }

            //statusText.text = "Finished";
        }
    }
}

[Serializable]
public class InfoSteps 
{ 
    public List<InfoStatus> steps;
}

[Serializable]
public class InfoStatus
{
    public List<InfoCar> Cars;
}

//[Serializable]
//public class InfoCar
//{
//    public int CarId;
//    public InfoPosition Position;
//    public string Direction;
//}

//[Serializable]
//public class InfoPosition
//{
//    public float x;
//    public float y;
//    public float z;
//}