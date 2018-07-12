using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Array of meshes to represent LEO data
//What will make this slow is the set of MonoBehaviors representing individual sensors
public class Slope : MonoBehaviour 
{
    GameObject sensor;
    List<GameObject> sensors;
    float[] tempData;
    float[] waterData;

    public const int LEO_X_MIN = 2, LEO_X_MAX = 28, LEO_Y_MIN = -5, LEO_Y_MAX = 5, LEO_Z_MIN = 1, LEO_Z_MAX = 5;
    public int xRes = 14, zRes = 11, yRes = 5;
    public const int SENSOR_COUNT = 496, DEPTH_1 = 154, DEPTH_2 = 154, DEPTH_3 = 76, DEPTH_4 = 78, DEPTH_5 = 34;
    //We need to code the array of 770 elements so that we can index "non-standard" layers without perfect sensor coverage.
    //The last 154*2 = 308 elements of the array should be fully intialized. The last 154 elements can be covered by a plant.

    void Awake()
    {
        sensors = new List<GameObject>();
        sensor = Resources.Load("Prefabs/Sensor") as GameObject;
    }

	// Use this for initialization
	void Start () 
    {
        GenerateDummySlope();
        
	}

    // Update is called once per frame
    void Update() 
    {
		
	}

    void GenerateDummySlope()
    {
        float dataMin = 1f;
        float dataMax = 5f;
        tempData = new float[770];
        waterData = new float[770];
        for (int i = 0; i < 770; i++)   //Assumes very particular array structure - must be crafted in R.
        {
            GameObject newSensor = GameObject.Instantiate(sensor);
            newSensor.transform.parent = this.transform;
            sensors.Add(newSensor);
            Sensor slopeSensor = newSensor.GetComponent<Sensor>();
            int row = Mathf.FloorToInt(i / (xRes));
            int col = Mathf.FloorToInt(i / (zRes));
            int depth = Mathf.FloorToInt(i / (xRes * zRes));

            float x = (i - (row * xRes)) - 6.5f;
            float z = row - (depth * zRes) - 5;
            float y = depth - 2;
            slopeSensor.LEO_Pos = new Vector3(x, y, z);
            tempData[i] = (Random.Range(dataMin, dataMax)-dataMin)/(dataMax-dataMin);
            waterData[i] = (Random.Range(dataMin, dataMax) - dataMin) / (dataMax - dataMin);
            slopeSensor.TempData = tempData[i];
            slopeSensor.WaterData = tempData[i];
            slopeSensor.SlopeIndex = i;
            slopeSensor.SizeFilter();
            slopeSensor.ColorFilter();

            if (i > 616)
            {
                if (Random.Range(0f, 1f) < 0.1f)
                {
                    GameObject plant = GameObject.Instantiate(Resources.Load("Prefabs/Plant")) as GameObject;
                    slopeSensor.SlopePlant = plant.GetComponent<Plant>();
                    plant.GetComponent<Emitter>().enabled = false;
                    plant.GetComponent<Plant>().enabled = false;
                }
            }
        }
    }
}
