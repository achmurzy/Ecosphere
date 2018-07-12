using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Emitter : MonoBehaviour {

    public Bounds SpatialExtent;
    public Vector3 LowerEmissionTrajectory = -Vector3.one, UpperEmissionTrajectory = Vector3.one;
    public float EmissionRate = 0.1f;
    public float EmissionForce = 5f;

    public GameObject Molecule, Flux;
    public float Lifetime = 5f;
    public bool exchanging, emitting;

    public class EmitterPackage { public FluxRibbon flux; public Collider other; public EmitterPackage(FluxRibbon ff, Collider o) { flux = ff; other = o; } }

    void Awake()
    {
        if (GetComponent<Collider>() == null)
            Debug.LogError("No collider on emitter object");
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
        newMol.transform.position = this.transform.position + SpatialExtent.center + (this.transform.right * Random.Range(SpatialExtent.min.x, SpatialExtent.max.x)) + 
            (this.transform.up * Random.Range(SpatialExtent.min.y, SpatialExtent.max.y)) + (this.transform.forward * Random.Range(SpatialExtent.min.z, SpatialExtent.max.z));
        
        Rigidbody molBody = newMol.GetComponent<Rigidbody>();
        Vector3 trajectory = (this.transform.right * Random.Range(LowerEmissionTrajectory.x, UpperEmissionTrajectory.x)) +
            (this.transform.up * Random.Range(LowerEmissionTrajectory.y, UpperEmissionTrajectory.y)) + (this.transform.forward * Random.Range(LowerEmissionTrajectory.z, UpperEmissionTrajectory.z));

        molBody.AddForce(trajectory.normalized * EmissionForce);
        molBody.AddTorque(Random.insideUnitSphere);

        newMol.GetComponent<Molecule>().exchanger = this;
        newMol.GetComponent<Molecule>().Lifetime = this.Lifetime;
        newMol.transform.SetParent(FindObjectOfType<EcosystemController>().transform, true);
    }

    void AddFlux()
    {
        GameObject newFlux = GameObject.Instantiate(Flux) as GameObject;
        newFlux.transform.position = this.transform.position + SpatialExtent.center + (this.transform.right.normalized * Random.Range(SpatialExtent.min.x, SpatialExtent.max.x)) +
            (this.transform.up.normalized * Random.Range(SpatialExtent.min.y, SpatialExtent.max.y)) + (this.transform.forward.normalized * Random.Range(SpatialExtent.min.z, SpatialExtent.max.z));
        Vector3 trajectory = (this.transform.right * Random.Range(LowerEmissionTrajectory.x, UpperEmissionTrajectory.x)) +
            (this.transform.up * Random.Range(LowerEmissionTrajectory.y, UpperEmissionTrajectory.y)) + (this.transform.forward * Random.Range(LowerEmissionTrajectory.z, UpperEmissionTrajectory.z));
        
        newFlux.transform.SetParent(FindObjectOfType<EcosystemController>().transform, true);
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

    public void TriggerEnterLogic(FluxRibbon flux, Collider other)
    {
        EmitterPackage payload = new EmitterPackage(flux, other);
        SendMessage("TriggerEnter", payload, SendMessageOptions.RequireReceiver);
    }

    public void TriggerExitLogic(FluxRibbon flux, Collider other)
    {
        EmitterPackage payload = new EmitterPackage(flux, other);
        SendMessage("TriggerExit", payload, SendMessageOptions.RequireReceiver);
    }
}
