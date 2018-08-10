using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEOController : EcosystemController 
{
    public const string WATER_LAYER = "Soil", VEGETATION_LAYER = "Vegetation", TEMPERATURE_LAYER = "Atmosphere";
    public Toggle WaterToggle, TemperatureToggle, VegetationToggle;
    public Slider InputSlider, TransferSlider;

    public float InputIntensity, TransferRate;
    public Slope East, West, Center, FocusSlope;

    void Awake()
    {
        base.Awake();
        //LEOParser.ParseCSV("MiniLEO_Temperature.csv");
    }

	// Use this for initialization
	void Start () 
    {
        base.Start();
        ToggleColorFilter();
	}
	
	// Update is called once per frame
	void Update () 
    {
        base.Update();
	}

    public void SetInputRate()
    {
        InputIntensity = InputSlider.value;
    }

    public void SetTransferRate()
    {
        TransferRate = TransferSlider.value;
    }

    public void ToggleColorFilter()
    {
        if (TemperatureToggle.isOn)
        {
            WaterToggle.isOn = false;
            Center.StartCoroutine("DayCycle");
            foreach (Sensor ss in Center.GetComponentsInChildren<Sensor>())
            {
                ss.ColorFilter();
            }
        }
        else
        {
            foreach (Sensor ss in Center.GetComponentsInChildren<Sensor>())
            {
                ss.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    public void ToggleSizeFilter()
    {
        if (WaterToggle.isOn)
        {
            TemperatureToggle.isOn = false;
            Center.StartCoroutine("RainCycle");
            foreach (Sensor ss in Center.GetComponentsInChildren<Sensor>())
            {
                ss.SizeFilter();
            }
        }
        else
        {
            foreach (Sensor ss in Center.GetComponentsInChildren<Sensor>())
            {
                ss.transform.localScale = Vector3.one;
            }
        }
    }

    public override void EcosystemRaycastHandler(RaycastHit info)
    {
        Sensor obj = info.collider.gameObject.GetComponent<Sensor>();
        if (obj != null)
        {
            //obj.SensorColorFlash();
        }
        else
        {
            //base.EcosystemRaycastHandler(info);
        }
    }

    public override bool CheckLayerEnabled(string layer)
    {
        switch (layer)
        {
            case WATER_LAYER:
                return WaterToggle.isOn;
            case VEGETATION_LAYER:
                return VegetationToggle.isOn;
            case TEMPERATURE_LAYER:
                return TemperatureToggle.isOn;
            default:
                Debug.Log("Invalid default return: " + layer);
                return false;
        }
    }
}
