using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

//The Ocean needs to be an Aqua cube. Make it
public class OceanController : MonoBehaviour {

    public Emitter CO2_source;
    public Ocean ocean;

    void Awake()
    {
        CO2_source = GetComponentInChildren<Emitter>();
        CO2_source.SpatialExtent = Vector3.one * ocean.oceanSize;
        CO2_source.Molecule.GetComponent<Molecule>().MolecularScale = ocean.oceanSize / 100;
    }

    // Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
       
	}

    void OnEnable()
    {
        CO2_source.StartEmitter();
    }
    void OnDisable()
    {
        CO2_source.StopEmitter();
    }
}
