using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Array of meshes to represent LEO data
//Simulation of 3D cellular automata
public class Slope : MonoBehaviour 
{
    GameObject sensor;
    List<GameObject> sensors;
    float[] tempData;
    float[] waterData;

    public const float LEO_X_MIN = 2, LEO_X_MAX = 28, LEO_Y_MIN = -5, LEO_Y_MAX = 5, LEO_Z_MIN = 1, LEO_Z_MAX = 5;
    public const int SENSOR_COUNT = 496, DEPTH_1 = 154, DEPTH_2 = 154, DEPTH_3 = 76, DEPTH_4 = 78, DEPTH_5 = 34;

    public LEOController LEO;
    public int xRes = 14, zRes = 11, yRes = 5;
   
    public float AmbientHeat, FluidInput, FluidOutput;
    public const float HEAT_MIN = 0, HEAT_MAX = 1, FLUID_IN = 1, FLUID_OUT = 3f;

    public float DiurnalCycle = 0, RainfallCycle = 0;
    private float diurnalDirection = 1f, rainfallDirection = 1f;
    public const float DAY_SPEED = 0.1f, RAIN_SPEED = 0.25f;

    void Awake()
    {
        sensors = new List<GameObject>();
        sensor = Resources.Load("Prefabs/Sensor") as GameObject;
        LEO = FindObjectOfType<LEOController>();
    }

	// Use this for initialization
	void Start () 
    {
        AmbientHeat = HEAT_MIN;
        FluidInput = FLUID_IN;
        FluidOutput = FLUID_OUT;
        GenerateSlope();

        StartCoroutine("DayCycle");
	}

    // Update is called once per frame
    void Update() 
    {
		
	}

    IEnumerator DayCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(DAY_SPEED * LEO.InputIntensity);
            DiurnalCycle += Time.deltaTime * Mathf.Sign(diurnalDirection);
            AmbientHeat = Mathf.Lerp(HEAT_MIN, HEAT_MAX, DiurnalCycle);
            if (DiurnalCycle > 1f || DiurnalCycle < -1f)
            {
                DiurnalCycle = diurnalDirection;
                diurnalDirection *= -1;
            }
        }
    }

    IEnumerator RainCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(RAIN_SPEED * LEO.InputIntensity);
            RainfallCycle += Time.deltaTime * Mathf.Sign(rainfallDirection);
            FluidInput = Mathf.Lerp(FLUID_IN, 0, RainfallCycle);
            if (RainfallCycle > 1f)
            {
                RainfallCycle = rainfallDirection;
                rainfallDirection *= -1;
            }
        }
    }

    public GameObject GetSensor(int index)
    {
        if (index != -1)
            return sensors[index];
        else
            return null;

    }

    void GenerateSlope()
    {
        for (int i = 0; i < xRes*yRes*zRes; i++)
        {
            GameObject newSensor = GameObject.Instantiate(sensor);
            newSensor.transform.parent = this.transform;
            sensors.Add(newSensor);
            Sensor slopeSensor = newSensor.GetComponent<Sensor>();
            
            int row = Mathf.FloorToInt(i / (zRes));
            int col = Mathf.FloorToInt(i / (xRes));
            int depth = Mathf.FloorToInt(i / (xRes * zRes));

            float z = (i - (row * zRes));    
            float x = row - (depth * xRes);     
            float y = depth;                    
            slopeSensor.LEO_Pos = new Vector3(x - 5, y - 2, z - 6.5f);

            slopeSensor.SlopeIndex = i;
            
            slopeSensor.NeighborIndices[0] = i - 1;    //back 
            slopeSensor.NeighborIndices[1] = i + 1;    //front
            if (z == 0)
                slopeSensor.NeighborIndices[0] = -1;
            else if (z == zRes-1)
                slopeSensor.NeighborIndices[1] = -1;

            slopeSensor.NeighborIndices[2] = i - zRes;  //left
            slopeSensor.NeighborIndices[3] = i + zRes;  //right        
            if (x == 0)
                slopeSensor.NeighborIndices[2] = -1;
            else if(x == xRes-1)
                slopeSensor.NeighborIndices[3] = -1;

            slopeSensor.NeighborIndices[4] = i - (xRes * zRes); //bottom
            slopeSensor.NeighborIndices[5] = i + (xRes * zRes); //top
            if(y == 0)
                slopeSensor.NeighborIndices[4] = -1;
            else if(y == yRes-1)
                slopeSensor.NeighborIndices[5] = -1;

            slopeSensor.SensorSlope = this;
            slopeSensor.LEO = this.LEO;
            slopeSensor.SizeFilter();
            slopeSensor.ColorFilter();
        }
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
