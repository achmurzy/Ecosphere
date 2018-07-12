using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ocean : MonoBehaviour, IPhotosensitive 
{
    MeshCollider oceanCollider;
    OceanController oceanController;
    Reef coralReef;

    public float oceanSize = 1;
    public float OceanSize { get { return oceanSize * 5; } private set { oceanSize = value; } }

    public GameObject Seawater, Hotwater;
    Material seawaterMat;
    Color seawaterColor, hotColor;
    private Vector3 seaScaleMin, seaScaleMax;
    private Vector3 seaPosMin, seaPosMax;

    public Slider CO2_Slider;
    private bool coolOff = false;
    private float heatLerp;
    public const float HEAT_DISSIP = 0.002f, LIGHT_HEAT = 0.025f;
    public float OceanSurfaceTemperature 
    { 
        get { return heatLerp; } 
        set 
        { 
            heatLerp = Mathf.Clamp01(value); 
            Hotwater.transform.localScale = Vector3.Lerp(seaScaleMin, seaScaleMax+(Vector3.one * 0.01f), heatLerp);
            Hotwater.transform.localPosition = Vector3.Lerp(seaPosMin, seaPosMax, heatLerp);
            coralReef.Bleach();
            //Hotwater.GetComponent<MeshRenderer>().material.color = Color.Lerp(seawaterColor, Color.red, heatLerp);
        } 
    }

    private float particleLerp;
    public float CO2_Conc
    {
        get { return particleLerp; }
        set
        {
            particleLerp = value;
            CarbonEmitter.EmissionForce = Mathf.Lerp(PARTICLE_FORCE_MIN, PARTICLE_FORCE_MAX, particleLerp);
            CarbonEmitter.EmissionRate = Mathf.Lerp(PARTICLE_RATE_MIN, PARTICLE_RATE_MAX, particleLerp);
        }
    }

    public Emitter CarbonEmitter;
    public const float PARTICLE_ACIDIDTY = 0.075f, PARTICLE_RESIDENCE = 1f, PARTICLE_FORCE_MIN = 100f, PARTICLE_FORCE_MAX = 300f, PARTICLE_RATE_MIN = 0.2f, PARTICLE_RATE_MAX = 0.01f, DOWNWARD = 0.5f;

    void Awake()
    {
        oceanController = GetComponentInParent<OceanController>();
        coralReef = FindObjectOfType<Reef>();
    }

	// Use this for initialization
	void Start () 
    {
        GameObject sea = Resources.Load("Prefabs/Seawater") as GameObject;
        Seawater = GameObject.Find("Seawater");
        Hotwater = GameObject.Instantiate(sea);
        Hotwater.gameObject.name = "Hotwater";

        seaScaleMin = new Vector3(2 * OceanController.OCEAN_X, 0, 2 * OceanController.OCEAN_Z);
        seaScaleMax = new Vector3(2 * OceanController.OCEAN_X, 2 * OceanController.OCEAN_Y, 2 * OceanController.OCEAN_Z);

        seaPosMin = new Vector3(0, 2*OceanController.OCEAN_Y, 0);
        seaPosMax = new Vector3(0, OceanController.OCEAN_Y, 0);
        
        Seawater.transform.localScale = seaScaleMax;
        Seawater.transform.localPosition = seaPosMax;

        Hotwater.transform.SetParent(oceanController.transform, false);

        Hotwater.transform.localScale = seaScaleMin;
        Hotwater.transform.localPosition = seaPosMin;

        this.transform.localScale = new Vector3((1f / 5f) * OceanController.OCEAN_X, OceanController.OCEAN_Y, (1f / 5f) * OceanController.OCEAN_Z);
        this.transform.localPosition = new Vector3(0, (OceanController.OCEAN_Y * 2) + 0.01f, 0);

        CarbonEmitter = this.GetComponent<Emitter>();
        Bounds emissionBounds = new Bounds();
        emissionBounds.center = new Vector3(0, OceanController.OCEAN_Y, 0);
        emissionBounds.min = new Vector3(-OceanController.OCEAN_X, 1, -OceanController.OCEAN_Z);
        emissionBounds.max = new Vector3(OceanController.OCEAN_X, 1, OceanController.OCEAN_Z);
        CarbonEmitter.SpatialExtent = emissionBounds;
        CarbonEmitter.UpperEmissionTrajectory = new Vector3(DOWNWARD, 0, DOWNWARD);
        CarbonEmitter.LowerEmissionTrajectory = new Vector3(-DOWNWARD, -1, -DOWNWARD);

        GetComponent<BoxCollider>().size = new Vector3(OceanController.OCEAN_X, 0.1f, OceanController.OCEAN_Z);
        CarbonEmitter.EmissionForce = PARTICLE_FORCE_MIN;
        CarbonEmitter.EmissionRate = PARTICLE_RATE_MIN;
        this.GetComponent<Emitter>().StartExchanger();

        seawaterMat = Material.Instantiate(Resources.Load("Materials/Seawater") as Material);
        seawaterColor = seawaterMat.color;
        hotColor = Color.red;
        hotColor.a = seawaterColor.a;
        Hotwater.GetComponent<MeshRenderer>().material.color = hotColor;
        this.GetComponent<MeshRenderer>().material.color = seawaterColor;
	}
	
	// Update is called once per frame
	void Update () 
    {
        float cool = HEAT_DISSIP;
        if (coolOff)
            cool += HEAT_DISSIP;
        OceanSurfaceTemperature -= cool;
	}

    //Photosensitive response a function of CO2 balance, heat ocean surface by depth, causing distal branches of corals to Bleach
    public bool LightEnter(SolarRay ray)
    {
        if (!coolOff)
        {
            OceanSurfaceTemperature += (LIGHT_HEAT + (PARTICLE_ACIDIDTY * CO2_Conc));
            if (OceanSurfaceTemperature >= 0.99f)
                coolOff = true;
        }
        else if (OceanSurfaceTemperature <= 0.05f)
        {
            FindObjectOfType<Reef>().ClearReef();
            coolOff = false;
        }
        
        return false;
    }

    public void ClimateChange()
    {
        CO2_Conc = CO2_Slider.value;
    }

    public void OnCollisionEnter(Collision collision)
    {
        float prob =  1- CO2_Conc;
        float val = Random.Range(0f, 1f);
        if (val > prob)
        {
            //Let the molecule pass by making it a trigger
            collision.collider.isTrigger = true;
        }
        else
        {
            collision.collider.GetComponent<Molecule>().Lifespan -= PARTICLE_RESIDENCE;
            //collision.collider.attachedRigidbody.AddForce((-2)*collision.collider.attachedRigidbody.velocity);
        }
    }

        /*public void HeatOcean()
    {
        OceanSurfaceTemperature = HeatSlider.value;
        LerpOceanColor(OceanSurfaceTemperature);
        //BroadcastMessage("StressCoral", HeatSlider.value);
    }

    public void AcidifyOcean()
    {
        OceanAcidity += PARTICLE_ACIDIDTY;
        LerpSeawaterColor(OceanAcidity);
    }
    
        public void LerpOceanColor(float heatValue)
    {
        this.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.red, heatValue);
    }

    public void LerpSeawaterColor(float acidValue)
    {
        Seawater.GetComponent<MeshRenderer>().material.color = Color.Lerp(seawaterColor, acidColor, acidValue);
    }*/
}
