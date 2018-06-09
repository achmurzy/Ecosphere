using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Emitter : MonoBehaviour {

    public Vector3 SpatialExtent = Vector3.one;
    public Vector3 EmissionTrajectory = Vector3.one;
    public float EmissionRate = 0.1f;
    public float EmissionForce = 5f;

    public BoxCollider DestructionTrigger;
    public GameObject Molecule, Flux;
    public float Lifetime = 5f;
    public bool exchanging, emitting;

    void Awake()
    {
        DestructionTrigger = GetComponent<BoxCollider>();
    }

	// Use this for initialization
	void Start () {
        
	}

    IEnumerator Exchange()
    {
        while (true)
        {
            AddMolecule();
            yield return new WaitForSeconds(EmissionRate);
        }
    }

    IEnumerator Emit()
    {
        while (true)
        {
            AddFlux();
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
        newMol.transform.position = this.transform.position + (this.transform.right * Random.Range(-SpatialExtent.x, SpatialExtent.x)) + 
            (this.transform.up * Random.Range(-SpatialExtent.y, SpatialExtent.y)) + (this.transform.forward * Random.Range(-SpatialExtent.z, SpatialExtent.z));
        
        Rigidbody molBody = newMol.GetComponent<Rigidbody>();
        Vector3 planeOffset = new Vector3(Random.Range(-SpatialExtent.x, SpatialExtent.x), 0, Random.Range(-SpatialExtent.y, SpatialExtent.y));
        molBody.AddForce((this.transform.up+planeOffset) * EmissionForce);
        molBody.AddTorque(Random.insideUnitSphere);

        newMol.GetComponent<Molecule>().exchanger = this;
    }

    void AddFlux()
    {
        GameObject newFlux = GameObject.Instantiate(Flux) as GameObject;
        newFlux.transform.position = this.transform.position + (this.transform.right * Random.Range(-SpatialExtent.x, SpatialExtent.x)) +
            (this.transform.up * Random.Range(-SpatialExtent.y, SpatialExtent.y)) + (this.transform.forward * Random.Range(-SpatialExtent.z, SpatialExtent.z));
        Vector3 trajectory = (this.transform.right * Random.Range(-EmissionTrajectory.x, EmissionTrajectory.x)) +
            (this.transform.up * Random.Range(-EmissionTrajectory.y, EmissionTrajectory.y)) + (this.transform.forward * Random.Range(-EmissionTrajectory.z, EmissionTrajectory.z));
        newFlux.GetComponent<FluxRibbon>().StartFluxing(trajectory.normalized, this);
    }

    public void StartEmitter()
    {
        StartCoroutine("Emit");
        emitting = true;
    }

    public void StopEmitter()
    {
        StopCoroutine("Emit");
        emitting = false;
    }

    public void StartExchanger()
    {
        StartCoroutine("Exchange");
        exchanging = true;
    }

    public void StopExchanger()
    {
        StopCoroutine("Exchange");
        exchanging = false;
    }

    void OnEnable()
    {
        if (emitting)
            StartEmitter();
        if (exchanging)
            StartExchanger();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
