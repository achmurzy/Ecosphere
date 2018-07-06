using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForestController : EcosystemController 
{
    public const float FOREST_X = 10, FOREST_Y = 10;
  
    GameObject sol;

    public const string VEGETATION_LAYER = "Vegetation", SOIL_LAYER = "Soil", ATMOSPHERE_LAYER = "Atmosphere";
    public Toggle SoilToggle, VegetationToggle, AtmosphereToggle;

    void Awake()
    {
        base.Awake();
    }

	// Use this for initialization
	void Start () 
    {
        base.Start();
        sol = GameObject.Find("Sol");
    }
	
	// Update is called once per frame
	void Update () 
    {
        base.Update();
    }

    public override bool CheckLayerEnabled(string layer)
    {
        switch (layer)
        {
            case SOIL_LAYER:
                return SoilToggle.isOn;
            case VEGETATION_LAYER:
                return VegetationToggle.isOn;
            case ATMOSPHERE_LAYER:
                return AtmosphereToggle.isOn;
            default:
                Debug.Log("Invalid default return: " + layer);
                return false;
        }
    }

    public override void EcosystemRaycastHandler(GameObject obj)
    {
        base.EcosystemRaycastHandler(obj);
    }
}
