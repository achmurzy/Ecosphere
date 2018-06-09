using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour {

    public Emitter exchanger;
    public Vector3 BirthPosition { get; private set; }
    public float MolecularScale = 0.05f;
    public float Lifespan { get; private set;  }
    
    void Start () 
    {
        this.transform.localScale = Vector3.one * MolecularScale;
        Lifespan = 0;
        BirthPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Lifespan += Time.deltaTime;
        if (Lifespan > exchanger.Lifetime)
            GameObject.Destroy(this);
	}

    void OnTriggerExit(Collider other)
    {
        if (other.transform == exchanger.transform)
        {
            GameObject.Destroy(this);
            Debug.Log("Molecule Destroyed by exit");
        }
    }
}
