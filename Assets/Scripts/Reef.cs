using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reef : MonoBehaviour {

    GameObject coral;
    List<Coral> reef;
    public const int REEF_CAPACITY = 25;
    public const float SUCCESSION_RATE = 0.5f;
    public Ocean oceanSurface;

    void Awake()
    {
        coral = Resources.Load("Prefabs/Coral") as GameObject;
        reef = new List<Coral>();
        oceanSurface = FindObjectOfType<Ocean>();
    }

	// Use this for initialization
	void Start () 
    {
        GetComponent<BoxCollider>().size = new Vector3(OceanController.OCEAN_X, 0.05f, OceanController.OCEAN_Z)*2f;
        StartCoroutine("Succession");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Succession()
    {
        while (true)
        {
            yield return new WaitForSeconds(SUCCESSION_RATE);

            if (reef.Count < REEF_CAPACITY/2)
            {
                Vector3 spot = new Vector3(Random.Range(-OceanController.OCEAN_X, OceanController.OCEAN_X), 0, Random.Range(-OceanController.OCEAN_Z, OceanController.OCEAN_Z));
                PlaceCoral(spot);
            }
        }
    }

    public void PlaceCoral(Vector3 point, bool local = false)
    {
        GameObject reefCoral = GameObject.Instantiate(coral);
        reefCoral.transform.parent = this.gameObject.transform;
        if(local)
            reefCoral.transform.localPosition = point;
        else
            reefCoral.transform.position = point;
        reefCoral.transform.localRotation = Quaternion.identity;
        Coral newCoral = reefCoral.GetComponent<Coral>();
        newCoral.StressCoral(oceanSurface.OceanSurfaceTemperature, this);
        AddCoral(newCoral);
    }

    public void Bleach()
    {
        foreach (Coral cc in reef)
        {
            cc.StressCoral(oceanSurface.OceanSurfaceTemperature, this);
        }
    }

    public void ClearReef()
    {
        StopCoroutine("Succession");
        foreach (Coral cc in reef)
        {
            GameObject.Destroy(cc.gameObject);
        }
        reef.Clear();
        StartCoroutine("Succession");
    }

    public void AddCoral(Coral newCoral)
    {
        reef.Add(newCoral);
    }

    public void RemoveCoral(Coral oldCoral)
    {
        reef.Remove(oldCoral);
    }

    void OnEnable()
    {
        StartCoroutine("Succession");
    }

    void OnDisable()
    {
        StopCoroutine("Succession");
    }
}
