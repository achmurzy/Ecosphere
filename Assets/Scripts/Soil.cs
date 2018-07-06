using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour, IPhotosensitive 
{
    Emitter soilWater;
	// Use this for initialization
	void Start () 
    {
        this.transform.localScale = new Vector3(ForestController.FOREST_X, 1, ForestController.FOREST_Y) / 5;
        Bounds emissionBounds = new Bounds();
        emissionBounds.center = new Vector3(0, -0.5f, 0);
        emissionBounds.min = new Vector3(-ForestController.FOREST_X, 0, -ForestController.FOREST_Y);
        emissionBounds.max = new Vector3(ForestController.FOREST_X, 0, ForestController.FOREST_Y);

        soilWater = this.GetComponent<Emitter>();
        soilWater.SpatialExtent = emissionBounds;
        soilWater.DestructionTrigger.size = new Vector3(ForestController.FOREST_X, 1, ForestController.FOREST_Y);
        soilWater.EmissionForce = 0;
        soilWater.EmissionRate = 0.5f;
        soilWater.StartEmitter();	
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public bool LightEnter(SolarRay ray)
    {
        //Spit out h2O
        return true;
    }
}
