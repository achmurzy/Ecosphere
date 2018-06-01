using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class GasExchanger : MonoBehaviour {

    //AnchorStageBehaviour anchor;

    List<GameObject> gases, destroyed;

    public float SpatialExtent = 5;
    public float EmissionRate = 0.1f;
    public float EmissionForce = 5f;
    public float DestructionDistance = 25f;
    public GameObject Molecule;

    void Awake()
    {
        //anchor = GetComponentInParent<AnchorStageBehaviour>();
        gases = new List<GameObject>();
        destroyed = new List<GameObject>();
    }

	// Use this for initialization
	void Start () {
        
	}

    IEnumerator Emit()
    {
        while (true)
        {
            AddMolecule();
            foreach (GameObject go in gases)
            {
                if (Vector3.Distance(go.transform.position, this.transform.position) > DestructionDistance)
                {
                    destroyed.Add(go);
                }
            }
            foreach (GameObject go in destroyed)
            {
                gases.Remove(go);
                GameObject.Destroy(go);
            }
            yield return new WaitForSeconds(EmissionRate);
        }
    }

	// Update is called once per frame
	void Update () 
    {
        
	}

    void AddMolecule()
    {
        GameObject newMol = GameObject.Instantiate(Molecule) as GameObject;
        newMol.transform.parent = this.transform;
        newMol.transform.localPosition = new Vector3(Random.Range(-SpatialExtent, SpatialExtent), Random.Range(-1f, 0), Random.Range(-SpatialExtent, SpatialExtent));
        
        Rigidbody molBody = newMol.GetComponent<Rigidbody>();
        Vector3 planeOffset = new Vector3(Random.Range(-SpatialExtent, SpatialExtent), 0, Random.Range(-SpatialExtent, SpatialExtent));
        molBody.AddForce((this.transform.up+planeOffset) * EmissionForce);
        molBody.AddTorque(Random.insideUnitSphere);
        
        gases.Add(newMol);
    }

    public void StartEmitter()
    {
        StartCoroutine("Emit");
    }

    public void StopEmitter()
    {
        StopCoroutine("Emit");
    }
}
