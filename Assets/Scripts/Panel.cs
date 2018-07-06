using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour, ITouchable 
{
    Emitter heatEmitter;
    public const float HEAT_FLUX_RATE = 0.5f, HEAT_FLUX_FORCE = 0.1f;

    void Awake()
    {
        heatEmitter = GetComponent<Emitter>();
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

        heatEmitter.DestructionTrigger.size = new Vector3(1, 100, 1);
        heatEmitter.EmissionForce = HEAT_FLUX_FORCE;
        heatEmitter.EmissionRate = HEAT_FLUX_RATE;
        heatEmitter.StartEmitter();
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public string Touch()
    {
        return "Solar panel";
    }
}
