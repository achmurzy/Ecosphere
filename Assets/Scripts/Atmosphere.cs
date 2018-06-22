using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour 
{
    Emitter greenhouseGases;
    Ocean ocean;

    void Awake()
    {
        ocean = FindObjectOfType<Ocean>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDisable()
    {
        Emitter em = ocean.GetComponentInChildren<Emitter>();
        em.StopExchanger();
    }

    void OnEnable()
    {
        Emitter em = ocean.GetComponentInChildren<Emitter>();
        em.StopExchanger();
    }
}
