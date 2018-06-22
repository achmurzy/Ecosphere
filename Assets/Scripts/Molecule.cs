using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour {

    public Emitter exchanger;
    public Vector3 BirthPosition { get; private set; }
    public float MolecularScale = 0.05f;
    public float Lifespan { get; private set;  }
    public float Lifetime { get; set; }
    
    void Start () 
    {
        this.transform.localScale = Vector3.one * MolecularScale;
        Lifespan = 0;
        BirthPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Lifespan += Time.deltaTime;
        if (Lifespan > Lifetime)
            GameObject.Destroy(this.gameObject);
	}

    void OnTriggerExit(Collider other)
    {
        if (exchanger != null)
        {
            if (other.transform == exchanger.transform)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
        else
            GameObject.Destroy(this.gameObject);
    }
}
