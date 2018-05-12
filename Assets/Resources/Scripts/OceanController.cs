using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class OceanController : MonoBehaviour {

    MeshCollider oceanCollider;

    public Slider heatSlider;
    public GasExchanger CO2_source;

    void Awake()
    {

    }

    // Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        this.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.red, heatSlider.value);
	}

    public void OnCollisionEnter(Collision collision)
    {
        float prob = heatSlider.value;
        float val = Random.RandomRange(0f, 1f);
        if (val > prob)
        {
            //Let the molecule pass by making it a trigger
            collision.collider.isTrigger = true;
        }
        else
        {
            //collision.collider.attachedRigidbody.AddForce((-2)*collision.collider.attachedRigidbody.velocity);
        }
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
