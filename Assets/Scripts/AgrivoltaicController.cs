using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgrivoltaicController : EcosystemController 
{
    public SolController Sol;
    public Agriculture Plants;
    public PanelController Panels;
    public const float AGRIVOLT_X = 10, AGRIVOLT_Y = 10;

    public const string VOLTAIC_LAYER = "Soil", VEGETATION_LAYER = "Vegetation", ATMOSPHERE_LAYER = "Atmosphere";
    public Toggle VoltaicToggle, VegetationToggle, AtmosphereToggle;

    public const float ENERGY_LOSS = 0.001f, WATER_PUMP = 0.01f;
    private float charge, yield, water;
    public float SolarCharge { get { return charge; } set { charge = value; BatteryCharge.GetComponent<RectTransform>().sizeDelta = new Vector2(BatteryWidth, value * BatteryHeight); } }
    public float AgriYield { get { return yield; } set { yield = value; } }
    public float WaterUse { get { return water; } set { water = value; } }

    public Image BatteryCharge;
    public float BatteryHeight, BatteryWidth;

    void Awake()
    {
        base.Awake();
        Sol = FindObjectOfType<SolController>();
        Plants = FindObjectOfType<Agriculture>();
        Panels = FindObjectOfType<PanelController>();

        Vector2 sizeD = BatteryCharge.GetComponent<RectTransform>().sizeDelta;
        BatteryHeight = sizeD.y;
        BatteryWidth = sizeD.x;

        SolarCharge = 0f;
        AgriYield = 0f;
        WaterUse = 0f;

        ParameterFunction1 = Panels.RotatePanels;
        ParameterFunction2 = Sol.AttenuateLight;
    }

	// Use this for initialization
	void Start () 
    {
        base.Start();
        Sol.Diurnal();
	}

    // Update is called once per frame
    void Update() 
    {
        base.Update();
        SolarCharge = Mathf.Clamp01(SolarCharge - ENERGY_LOSS);
        WaterUse = Mathf.Clamp01(WaterUse + WATER_PUMP);
	}

    public override void EcosystemRaycastHandler(RaycastHit info)
    {
        GameObject obj = info.collider.gameObject;
        if (obj == Plants.gameObject)
        {
            Plants.PlacePlant(info.point);
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
            case VOLTAIC_LAYER:
                return VoltaicToggle.isOn;
            case VEGETATION_LAYER:
                return VegetationToggle.isOn;
            case ATMOSPHERE_LAYER:
                return AtmosphereToggle.isOn;
            default:
                Debug.Log("Invalid default return: " + layer);
                return false;
        }
    }

    public void Night()
    {
        foreach (Panel pp in GetComponentsInChildren<Panel>())
        {
            pp.Energy = 0;
        }
    }

    public void ChargeBattery(float batteryCharge)
    {
        SolarCharge = Mathf.Clamp01(SolarCharge + batteryCharge);
    }
}
