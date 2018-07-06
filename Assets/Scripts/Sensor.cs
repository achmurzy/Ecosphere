using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour, ITouchable
{
    public const float SENSOR_SCALE_MIN = 0.1f, SENSOR_SCALE_MAX = 0.5f;
    private Material sensorMat;
    public Color lowColor = Color.blue, highColor = Color.red;

    private Vector3 leo_pos;
    public Vector3 LEO_Pos { get { return leo_pos; } set { leo_pos = value; this.transform.localPosition = new Vector3(2*leo_pos.x, leo_pos.y, LEO_Pos.z); } }
    private float tempData, waterData; 
    public float TempData { get { return tempData; } 
        set 
        { 
            tempData = value; 
        }
    }
    public float WaterData
    {
        get { return waterData; }
        set
        {
            waterData = value;
        }
    }
    public int SlopeIndex {get; set;}
    public string Touch() { return "LEO Sensor"; }

    public Plant SlopePlant { get { return this.GetComponentInChildren<Plant>(); } set { value.transform.position = this.transform.position + this.transform.up; value.transform.parent = FindObjectOfType<LEOController>().transform; } }

    void Awake()
    {
        sensorMat = Material.Instantiate(Resources.Load("Materials/Sensor") as Material);
    }

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public void SizeFilter()
    {
        this.transform.localScale = Vector3.one * Mathf.Lerp(SENSOR_SCALE_MIN, SENSOR_SCALE_MAX, waterData);
    }

    public void ColorFilter()
    {
        Color datColor = Color.Lerp(lowColor, highColor, tempData);
        //datColor.a = tempData;
        this.GetComponent<MeshRenderer>().material.color = datColor;
    }
}
