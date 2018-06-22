using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class OceanController : EcosystemController 
{
    public const float OCEAN_X = 10, OCEAN_Y = 2, OCEAN_Z = 10;
    public const string CORAL_LAYER = "Vegetation", ATMOSPHERE_LAYER = "Atmosphere", OCEAN_LAYER = "Soil";

    public Toggle OceanToggle, CoralToggle, AtmosphereToggle;

    void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start()
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
            case OCEAN_LAYER:
                return OceanToggle.isOn;
            case CORAL_LAYER:
                return CoralToggle.isOn;
            case ATMOSPHERE_LAYER:
                return AtmosphereToggle.isOn;
            default:
                Debug.Log("Invalid default return: " + layer);
                return false;
        }
    }
}
