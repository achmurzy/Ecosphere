using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agriculture : MonoBehaviour {

    private List<Plant> plants;
    private GameObject plant, water;

    public const int PLANT_COUNT_MAX = 15;

    void Awake()
    {
        plant = Resources.Load("Prefabs/Plant") as GameObject;
        water = Resources.Load("Prefabs/SoilWater") as GameObject;
        plants = new List<Plant>();
    }

	// Use this for initialization
	void Start () 
    {
        //GetComponent<BoxCollider>().size = new Vector3(AgrivoltaicController.AGRIVOLT_X, 0.1f, AgrivoltaicController.AGRIVOLT_Y);
        for (int i = 0; i < (PLANT_COUNT_MAX/2); i++)
        {
            Vector3 spot = new Vector3(Random.Range(AgrivoltaicController.AGRIVOLT_X * -2, AgrivoltaicController.AGRIVOLT_X * 2),
                          0, Random.Range(AgrivoltaicController.AGRIVOLT_Y * -2, AgrivoltaicController.AGRIVOLT_Y * 2));
            PlacePlant(spot);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        		
	}

    public void PlacePlant(Vector3 point)
    {
        if (plants.Count < PLANT_COUNT_MAX)
        {
            GameObject agri = GameObject.Instantiate(plant);
            Plant pp = agri.GetComponent<Plant>();
            foreach (Plant op in plants)
            {
                if (op.bush.Radius > Vector3.Distance(op.transform.position, agri.transform.position))
                {
                    GameObject.Destroy(agri);
                    return;
                }
            }

            agri.transform.SetParent(this.transform, false);
            agri.transform.position = point;
            

            GameObject soilWater = GameObject.Instantiate(water);
            soilWater.transform.SetParent(this.transform, false);

            pp.SetWater(soilWater);
            plants.Add(pp);
        }
    }

    public void RemovePlant(Plant pp)
    {
        plants.Remove(pp);
    }
}
