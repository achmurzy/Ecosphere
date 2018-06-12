using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need to write a coral growth routine
public class Coral : MonoBehaviour {

    public float CoralSize { get; set; }
    Material coralMat;
    
    ParticleSystem xanth;

    void Awake()
    {
        coralMat = Material.Instantiate(Resources.Load("Materials/Coral") as Material);
        xanth = GetComponentInChildren<ParticleSystem>();
        //xanth.main.startColor = coralMat.color;
        
    }

	// Use this for initialization
	void Start () {
        CoralSize = 1;
        foreach (Transform tt in GetComponentsInChildren<Transform>())
            tt.GetComponent<MeshRenderer>().material = coralMat;
	}
	
	// Update is called once per frame
	void Update () {
        //this.transform.position = new Vector3(0, CoralSize, 0);
	}
}
