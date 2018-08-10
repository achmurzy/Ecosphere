using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour, ITouchable
{
    public Slope SensorSlope;
    public LEOController LEO;

    public const float SENSOR_SCALE_MIN = 0.1f, SENSOR_SCALE_MAX = 1f;
    private Material sensorMat;
    public Color lowColor = Color.blue, highColor = Color.red;

    private Vector3 leo_pos;
    public Vector3 LEO_Pos { get { return leo_pos; } set { leo_pos = value; this.transform.localPosition = new Vector3(leo_pos.x, leo_pos.y, 2*leo_pos.z); } }
    
    private float tempData, waterData; 
    public float TempData { get { return tempData; } 
        set 
        { 
            tempData = value; 
        }
    }
    public const float HEAT_XFER = 0.05f;

    public float WaterData
    {
        get { return waterData; }
        set
        {
            waterData = value;
        }
    }
    public const float FLUID_FLOW = 0.01f, BACKWARD_FLOW = 0.9f;

    public int SlopeIndex {get; set;}
    public string Touch() { return "LEO Sensor"; }

    public Plant SlopePlant { get { return this.GetComponentInChildren<Plant>(); } set { value.transform.position = this.transform.position + this.transform.up; value.transform.parent = FindObjectOfType<LEOController>().transform; } }
    public int[] NeighborIndices;

    void Awake()
    {
        sensorMat = Material.Instantiate(Resources.Load("Materials/Sensor") as Material);
        NeighborIndices = new int[6];
    }

	// Use this for initialization
	void Start () 
    {
		
	}

	void Update () 
    {
        if (NeighborIndices[5] == -1)
        {
            float externalTemp = SensorSlope.AmbientHeat;
            float externalFluid = SensorSlope.FluidInput;

            tempData += (externalTemp - tempData) * HEAT_XFER;
            waterData += externalFluid * FLUID_FLOW;
        }

        if(LEO.WaterToggle.isOn)
            FluidUpdate();
        else if(LEO.TemperatureToggle.isOn)
             HeatUpdate();
	}

    //Flow to indices - 3,4 - forward and bottom
    private void FluidUpdate()
    {
        float fluidDist = waterData * FLUID_FLOW * LEO.TransferRate;
        
        bool back = NeighborIndices[0] == -1, bottom = NeighborIndices[4] == -1;
        if (back)
        {
            if (bottom)
            {
                float outFlow = SensorSlope.FluidOutput;
                waterData -= outFlow * FLUID_FLOW;
            }
        }
        else
        {
            Sensor backward = SensorSlope.GetSensor(NeighborIndices[0]).GetComponent<Sensor>();
            backward.WaterData += fluidDist * BACKWARD_FLOW;
            this.waterData -= fluidDist * BACKWARD_FLOW;
        }

        if (!bottom)
        {
            Sensor below = SensorSlope.GetSensor(NeighborIndices[4]).GetComponent<Sensor>();
            below.WaterData += fluidDist;
            this.waterData -= fluidDist;
        }
        SizeFilter();
    }

    //Check all neighbors, and transfer a proportion of heat to cooler blocks
    private void HeatUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            if (NeighborIndices[i] != -1)
            {
                Sensor neighbor = SensorSlope.GetSensor(NeighborIndices[i]).GetComponent<Sensor>();
                if (neighbor.TempData < this.tempData) //Important that these inequalities converge - thermodynamic equilibrium
                {
                    float heatDist = (this.tempData - neighbor.TempData) * HEAT_XFER * LEO.TransferRate;
                    neighbor.TempData += heatDist;
                    this.tempData -= heatDist;
                }
            }
        }
        ColorFilter();
    }

    public void SizeFilter()
    {
        this.transform.localScale = Vector3.one * Mathf.Lerp(SENSOR_SCALE_MIN, SENSOR_SCALE_MAX, waterData);
        this.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void ColorFilter()
    {
        Color datColor = Color.Lerp(lowColor, highColor, tempData);
        //datColor.a = tempData;
        this.GetComponent<MeshRenderer>().material.color = datColor;
    }

    public void SensorColorFlash()
    { 
        this.GetComponent<MeshRenderer>().material.color = Color.green;
        for (int i = 0; i < 6; i++)
        {
            GameObject sensor = this.SensorSlope.GetSensor(NeighborIndices[i]);
            if(sensor!=null)
                sensor.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
