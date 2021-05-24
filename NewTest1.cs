using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour
{
    int count = 0;

    [SerializeField]
    private Button EventButton;

    [SerializeField]
    private Button DestroyButton;

    public Text CountText = null;
    public Text CountText2 = null;

    GameObject[] TestArray = new GameObject[3];

    Transform[] SampleArray = new Transform[3];


    public GameObject TestCube;
    public GameObject TestCapsule;
    public GameObject TestCylinder;

    public GameObject TestSphere;

    GameObject[] gameObjects = new GameObject[3];

    float[] ArrayX = new float[3];
    float[] ArrayY = new float[3];
    float[] ArrayZ = new float[3];

    float[] ArrayXTo = new float[3];
    float[] ArrayYTo = new float[3];
    float[] ArrayZTo = new float[3];

    float[,] PositionArray = new float[3, 3];

    // Start is called before the first frame update
    private void Start()
    {
        TestArray[0] = TestCube;
        TestArray[1] = TestCapsule;
        TestArray[2] = TestCylinder;

        for (int i = 0; i < 3; i++)
        {
            gameObjects[i] = Instantiate(TestArray[i], TestArray[i].transform.position + new Vector3(30, 30, 30), TestArray[i].transform.rotation);
        }

        for (int i = 0; i < 3; i++)
        {
            SampleArray[i] = gameObjects[i].transform;
        }



        for (int i = 0; i < 3; i++)
        {
            ArrayX[i] = gameObjects[i].transform.position.x;
            ArrayY[i] = gameObjects[i].transform.position.y;
            ArrayZ[i] = gameObjects[i].transform.position.z;

        }

        ArrayXTo = (float[])ArrayX.Clone();
        ArrayYTo = (float[])ArrayX.Clone();
        ArrayZTo = (float[])ArrayX.Clone();

        for (int i = 0; i < 3; i++)
        {
            PositionArray[i, 0] = ArrayXTo[i];
            PositionArray[i, 1] = ArrayYTo[i];
            PositionArray[i, 2] = ArrayZTo[i];
        }

        /*TestArray[0].position = new Vector3(10, 10, 10);
        TestArray[1].position = new Vector3(20, 20, 20);
        TestArray[2].position = new Vector3(30, 30, 30);*/
        TestArray[0].transform.position = new Vector3(70, 70, 70);
        TestArray[1].transform.position = new Vector3(100, 100, 100);
        TestArray[2].transform.position = new Vector3(150, 150, 150);


    }

    private void Awake()
    {
        EventButton.onClick.AddListener(FuncButton);
        DestroyButton.onClick.AddListener(DetroyObject);
    }

    // Update is called once per frame
    void Update()
    {
    }



    void FuncButton()
    {

        /*CountText.GetComponent<Text>().text = "Count" + count.ToString() + " :  x : " + TestArray[3].position.x.ToString() + " :  y :  \n" + TestArray[3].position.y.ToString() + " :  z : " + TestArray[3].position.z.ToString();
        CountText2.GetComponent<Text>().text = "Count" + count.ToString() + " :  x : " + SampleArray[3].position.x.ToString() + " :  y :  \n" + SampleArray[3].position.y.ToString() + " :  z : " + SampleArray[3].position.z.ToString();*/

        if (EventButton.gameObject.activeSelf)
        {
            CountText.GetComponent<Text>().text = "TestArray's Index[" + count + "] = \n" + TestArray[count].transform.position.ToString();
            CountText2.GetComponent<Text>().text = "PositionArray's Index[" + count + "] = \n" + PositionArray[count, 0].ToString() + ", " + PositionArray[count, 1].ToString() + ", " + PositionArray[count, 2].ToString();
        }
        if (count == 2)
        {
            count = 0;
        }
        else if (count != 2)
        {
            count++;
        }
    }
    void DetroyObject()
    {
        if (DestroyButton.gameObject.activeSelf)
        {
            /*gameObjects[1].SetActive(false);*/
            Destroy(gameObjects[count]);
            Debug.LogWarning("Destroy" + count.ToString());
            if (count == 2)
            {
                count = 0;
            }
            else if (count != 2)
            {
                count++;
            }
        }
        else
        {
            Debug.LogWarning("Nothing");
        }
    }

}
