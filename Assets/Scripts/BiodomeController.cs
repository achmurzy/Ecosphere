using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiodomeController : EcosystemController 
{
    public LungController East, West;
    HabitatController Habitat;
    SolController Sol;

    public const string HABITAT_LAYER = "Vegetation", LUNG_LAYER = "Soil", ATMOSPHERE_LAYER = "Atmosphere";
    public Toggle HabitatToggle, LungToggle, AtmosphereToggle;

    void Awake()
    {
        base.Awake();

        East = GameObject.Find("East_Lung").GetComponent<LungController>();
        West = GameObject.Find("West_Lung").GetComponent<LungController>();
        Habitat = FindObjectOfType<HabitatController>();
        Sol = FindObjectOfType<SolController>();

        ParameterFunction1 = Sol.ChangePhotoperiod;
        ParameterFunction2 = LungPumpRate;
    }

	// Use this for initialization
	void Start () 
    {
        base.Start();

        Sol.Diurnal();
	}
	
	// Update is called once per frame
	void Update () 
    {
        base.Update();
	}

    public void Night()
    {
        Habitat.Deflate();
        East.Deflate();
        West.Deflate();
    }

    public void LungPumpRate(float val)
    {
        East.SlidePumpRate(val);
        West.SlidePumpRate(val);
    }

    public override bool CheckLayerEnabled(string layer)
    {
        switch (layer)
        {
            case HABITAT_LAYER:
                return HabitatToggle.isOn;
            case LUNG_LAYER:
                return LungToggle.isOn;
            case ATMOSPHERE_LAYER:
                return AtmosphereToggle.isOn;
            default:
                Debug.Log("Invalid default return: " + layer);
                return false;
        }    
    }
}
