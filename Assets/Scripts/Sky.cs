using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour 
{
    GameObject cloud;
    List<Cloud> firmament;
    private Stand stand;

    void Awake()
    {
        cloud = Resources.Load("Prefabs/Cloud") as GameObject;
        firmament = new List<Cloud>();
        stand = FindObjectOfType<Stand>();
    }
    
    // Use this for initialization
	void Start () 
    {
        	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Monsoon()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (firmament.Count < 5)
            {
                GameObject nimbus = GameObject.Instantiate(cloud);
                nimbus.transform.SetParent(this.gameObject.transform, false);
                nimbus.transform.localPosition = new Vector3(Random.RandomRange(-ForestController.FOREST_X, ForestController.FOREST_X), 
                        FindObjectOfType<SolController>().SolDistance / 2, Random.RandomRange(-ForestController.FOREST_Y, ForestController.FOREST_Y));
                firmament.Add(cloud.GetComponent<Cloud>());
            }
        }
    }

    void OnEnable()
    {
        StartCoroutine("Monsoon");
        //What happens if atmo is enabled while vegetation is disabled?
        /*foreach (Emitter em in stand.GetComponentsInChildren<Emitter>())
        {
            em.StartExchanger();
        }*/
    }

    void OnDisable()
    {
        StopCoroutine("Monsoon");
        //Disable all vegetation emitters, which create atmospheric components
        foreach (Emitter em in stand.GetComponentsInChildren<Emitter>())
        {
            em.StopExchanger();
        }
    }
}
