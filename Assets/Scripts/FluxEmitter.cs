using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class FluxEmitter : MonoBehaviour
{
    //AnchorStageBehaviour anchor;
    List<GameObject> fluxes, destroyed;

    public float SpatialExtent = 5;
    public float EmissionRate = 0.1f;
    public float DestructionDistance = 25f;
    public GameObject Flux;

    void Awake()
    {
        //anchor = GetComponentInParent<AnchorStageBehaviour>();
        fluxes = new List<GameObject>();
        destroyed = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
    {

    }

    IEnumerator Emit()
    {
        while (true)
        {
            AddFlux();
            foreach (GameObject go in fluxes)
            {
                if (Vector3.Distance(go.transform.position, this.transform.position) > DestructionDistance)
                {
                    destroyed.Add(go);
                }
            }
            foreach (GameObject go in destroyed)
            {
                fluxes.Remove(go);
                GameObject.Destroy(go);
            }
            yield return new WaitForSeconds(EmissionRate);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddFlux()
    {
        GameObject newFlux = GameObject.Instantiate(Flux) as GameObject;
        newFlux.transform.parent = this.transform;
        newFlux.transform.localPosition = new Vector3(Random.Range(-SpatialExtent, SpatialExtent), Random.Range(-1f, 0), Random.Range(-SpatialExtent, SpatialExtent));

        fluxes.Add(newFlux);
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
