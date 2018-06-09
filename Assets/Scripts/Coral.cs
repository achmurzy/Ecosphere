using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coral : MonoBehaviour {

    public float CoralSize { get; set; }

	// Use this for initialization
	void Start () {
        CoralSize = 1;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = new Vector3(0, CoralSize, 0);
	}
}
