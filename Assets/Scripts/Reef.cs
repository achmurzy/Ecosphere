using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reef : MonoBehaviour {

    GameObject coral;
    List<Coral> reef;
    public const int REEF_CAPACITY = 30, SUCCESSION_RATE = 1;
    public Slider HeatSlider;

    void Awake()
    {
        coral = Resources.Load("Prefabs/Coral") as GameObject;
        reef = new List<Coral>();
    }

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Succession()
    {
        while (true)
        {
            yield return new WaitForSeconds(SUCCESSION_RATE);
            if (reef.Count < REEF_CAPACITY)
            {
                GameObject reefCoral = GameObject.Instantiate(coral);
                reefCoral.transform.parent = this.gameObject.transform;
                reefCoral.transform.localPosition = new Vector3(Random.RandomRange(-OceanController.OCEAN_X, OceanController.OCEAN_X), 0, Random.RandomRange(-OceanController.OCEAN_Z, OceanController.OCEAN_Z));
                Coral newCoral = reefCoral.GetComponent<Coral>();
                newCoral.StressCoral(HeatSlider.value);
                reef.Add(newCoral);
            }
        }
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
