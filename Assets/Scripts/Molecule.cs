using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour {
    public float MolecularScale = 0.05f;
	// Use this for initialization
	
    void Start () 
    {
        this.transform.localScale = Vector3.one * MolecularScale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
