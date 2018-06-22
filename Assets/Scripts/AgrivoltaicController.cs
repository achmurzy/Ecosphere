using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgrivoltaicController : EcosystemController 
{
    public const float AGRIVOLT_X = 10, AGRIVOLT_Y = 10;

    public const string VOLTAIC_LAYER = "Soil", VEGETATION_LAYER = "Vegetation", ATMOSPHERE_LAYER = "Atmosphere";
    public Toggle VoltaicToggle, VegetationToggle, AtmosphereToggle;

    void Awake()
    {
        base.Awake();
    }

	// Use this for initialization
	void Start () 
    {
        base.Start();
	}

    // Update is called once per frame
    void Update() 
    {
        base.Update();	
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
}
