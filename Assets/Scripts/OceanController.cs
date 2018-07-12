using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class OceanController : EcosystemController 
{
    public const float OCEAN_X = 10, OCEAN_Y = 4, OCEAN_Z = 10;
    public const string CORAL_LAYER = "Vegetation", ATMOSPHERE_LAYER = "Atmosphere", OCEAN_LAYER = "Soil";

    public Toggle OceanToggle, CoralToggle, AtmosphereToggle;

    Reef Coral;

    void Awake()
    {
        base.Awake();
        Coral = FindObjectOfType<Reef>();
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

    public override void EcosystemRaycastHandler(RaycastHit info)
    {
        GameObject obj = info.collider.gameObject;
        if (obj == Coral.gameObject)
        {
            Coral.PlaceCoral(info.point);
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
