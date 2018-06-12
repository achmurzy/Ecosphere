using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour {

    Emitter heatEmitter;
    public const float HEAT_FLUX_RATE = 0.5f, HEAT_FLUX_FORCE = 0.1f;

    void Awake()
    {
        heatEmitter = GetComponentInChildren<Emitter>();
    }

	// Use this for initialization
	void Start () 
    {
        heatEmitter.EmissionTrajectory = new Vector3(0, 1, 0);
        heatEmitter.DestructionTrigger.size = new Vector3(1, 100, 1);
        heatEmitter.EmissionForce = HEAT_FLUX_FORCE;
        heatEmitter.EmissionRate = HEAT_FLUX_RATE;
        heatEmitter.SpatialExtent = heatEmitter.gameObject.transform.localScale/2;
        heatEmitter.StartEmitter();
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}
}
