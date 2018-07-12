using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour, ITouchable, IEmitter
{
    AgrivoltaicController avc;
    Emitter heatEmitter;
    
    private float energy;
    public float Energy { get { return energy; } 
        set 
        { 
            energy = Mathf.Clamp01(value);
            
        } }
    public const float LIGHT_QUANTUM = 0.05f, HEAT_QUANTUM = 0.1f;

    public const float HEAT_FLUX_RATE_MIN = 3.5f, HEAT_FLUX_RATE_MAX = 1.05f, HEAT_FLUX_FORCE = 0.1f;
    public const float HEAT_FLUX_INTERVAL = 0.1f, FLUXER_HEIGHT = 100;

    void Awake()
    {
        heatEmitter = GetComponent<Emitter>();
        avc = FindObjectOfType<AgrivoltaicController>();
    }

	// Use this for initialization
	void Start () 
    {
        Bounds emissionBounds = new Bounds();
        emissionBounds.min = -heatEmitter.gameObject.transform.localScale / 2;
        emissionBounds.max = heatEmitter.gameObject.transform.localScale / 2;
        heatEmitter.LowerEmissionTrajectory = new Vector3(0, 0, 0);
        heatEmitter.UpperEmissionTrajectory = new Vector3(0, 1, 0);
        heatEmitter.SpatialExtent = emissionBounds;

        BoxCollider bc = GetComponent<BoxCollider>();
        bc.center = new Vector3(0, FLUXER_HEIGHT / 2, 0);
        bc.size = new Vector3(1, FLUXER_HEIGHT, 1);
        
        heatEmitter.EmissionForce = HEAT_FLUX_FORCE;
        heatEmitter.EmissionRate = HEAT_FLUX_RATE_MIN;
        heatEmitter.StartEmitter();
	}
	
	void Update () 
    {

	}

    public void Volt()
    {
        //Add heat flux and charge battery
        Energy += HEAT_QUANTUM;
        float charge = LIGHT_QUANTUM - (LIGHT_QUANTUM * energy);
        avc.ChargeBattery(charge);

        if (heatEmitter.EmissionRate - HEAT_FLUX_INTERVAL > HEAT_FLUX_RATE_MAX)
            heatEmitter.EmissionRate -= HEAT_FLUX_INTERVAL;
    }

    public void Cool()
    {
        Energy -= HEAT_QUANTUM;
        if (heatEmitter.EmissionRate + HEAT_FLUX_INTERVAL < HEAT_FLUX_RATE_MIN)
            heatEmitter.EmissionRate += HEAT_FLUX_INTERVAL;
    }

    public string Touch()
    {
        return "Solar panel";
    }

    public void OnTriggerEnter(Collider other)
    {
        FluxRibbon fr = other.GetComponentInParent<FluxRibbon>();
        if (fr != null)
        {
            if (fr.Fluxer != GetComponent<Emitter>())
            {
                Cool();
                fr.Fluxed();
            }
        }
    }

    public void TriggerEnter(Emitter.EmitterPackage ep)
    {}

    public void TriggerExit(Emitter.EmitterPackage ep)
    {
        FluxRibbon fr = ep.flux;
        Collider other = ep.other;
        if (other.gameObject == this.gameObject)
        {
            fr.Fluxed();
        }
    }
}
