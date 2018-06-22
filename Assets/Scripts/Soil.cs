using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour, IPhotosensitive 
{

	// Use this for initialization
	void Start () 
    {
        this.transform.localScale = new Vector3(ForestController.FOREST_X, 1, ForestController.FOREST_Y) / 5;
        Bounds emissionBounds = new Bounds();
        emissionBounds.center = new Vector3(0, -0.5f, 0);
        emissionBounds.min = new Vector3(-ForestController.FOREST_X, 0, -ForestController.FOREST_Y);
        emissionBounds.max = new Vector3(ForestController.FOREST_X, 0, ForestController.FOREST_Y);
        this.GetComponent<Emitter>().SpatialExtent = emissionBounds;
        this.GetComponent<Emitter>().DestructionTrigger.size = new Vector3(ForestController.FOREST_X, 1, ForestController.FOREST_Y);
        this.GetComponent<Emitter>().EmissionForce = 0;
        this.GetComponent<Emitter>().StartEmitter();	
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public bool LightEnter()
    {
        //Spit out h2O
        return true;
    }
}
