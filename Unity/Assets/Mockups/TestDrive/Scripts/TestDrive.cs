using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrive : MonoBehaviour
{ 
    public Vector2 minMaxWidth;
    public Vector2 minMaxDepth;
    public float multiplier = 1;
    public float repeatTime = 1;
    public float speedPosition = 1;
    public float speedRotation = 1;
    private Vector3 targetPosition;
    private Vector3 targetRotation;

    enum Directions
    {
        Left,
        Front,
        Right
    }

    struct Option
    {
        public Vector3 position;
        public Directions direction;
        public float rotation;

    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Take", 1, repeatTime);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speedPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * speedRotation);
    }

    void Take()
    {
        Option[] options = Build(transform);
        if(options.Length < 1)
        {
            return;
        }
        int index = Random.Range(0, options.Length);
        Option option = options[index];

        targetPosition = option.position;
        targetRotation = new Vector3(0, option.rotation + targetRotation.y, 0);
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Quaternion.Euler(0, option.rotation, 0).eulerAngles);

        Debug.Log(option.direction.ToString());


        /*string[] names = new string[] { "Left", "Front", "Right" };
        int index = Random.Range(0, names.Length);

        Vector3[] positions = new Vector3[] { new Vector3(-1, 0, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 1) };
        float[] rotations = new float[] { -90, 0, 90 };

        Vector3 worldPosition = transform.TransformPoint(positions[index]);
        transform.position = worldPosition;

        Vector3 sum = transform.rotation.eulerAngles + Quaternion.Euler(0, rotations[index], 0).eulerAngles;
        transform.rotation = Quaternion.Euler(sum);

        Debug.Log(names[index]);*/
    }

    Option[] Build(Transform t)
    {
        List<Option> options = new List<Option>();
        Vector3[] possiblePositions = new Vector3[] { new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0) }; //Left, Front, Right
        float[] possibleRotations = new float[] { -90, 0, 90 };

        for(int i = 0; i < possiblePositions.Length; i++)
        {
            bool isin = true;
            Vector3 nextPosition = t.TransformPoint(possiblePositions[i] * multiplier);
            if(nextPosition.x < minMaxWidth.x || nextPosition.x > minMaxWidth.y)
            {
                isin = false;
            }
            if(nextPosition.z < minMaxDepth.x || nextPosition.z > minMaxDepth.y)
            {
                isin = false;
            }

            if(isin)
            {
                Option o;
                o.direction = (Directions)i; //cast
                o.position = nextPosition;
                o.rotation = possibleRotations[i];
                options.Add(o);
            }
        }

        return options.ToArray();
    }
}
