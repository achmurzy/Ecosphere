using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agriculture : MonoBehaviour {

    private List<Plant> plants;
    private GameObject plant;

    void Awake()
    {
        plant = Resources.Load("Prefabs/Plant") as GameObject;
        plants = new List<Plant>();
    }

	// Use this for initialization
	void Start () 
    {
        for (int i = 0; i < 10; i++)
        {
            PlacePlant();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlacePlant()
    {
        GameObject agri = GameObject.Instantiate(plant);
        agri.transform.SetParent(this.transform, false);
        agri.transform.localPosition = new Vector3(Random.RandomRange(AgrivoltaicController.AGRIVOLT_X * -2, AgrivoltaicController.AGRIVOLT_X * 2), 
                          0, Random.RandomRange(AgrivoltaicController.AGRIVOLT_Y * -2, AgrivoltaicController.AGRIVOLT_Y * 2));
        Plant pp = agri.GetComponent<Plant>();
        plants.Add(pp);
        //Potentially initialize stuff
    }
}
