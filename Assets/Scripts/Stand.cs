using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stand : MonoBehaviour 
{
    GameObject tree;
    List<Tree> stand;

    Emitter soilWater;

    public const int STAND_SIZE = 20;
    public const float SUCCESSION_RATE = 5f;

    void Awake()
    {
        tree = Resources.Load("Prefabs/Tree") as GameObject;
    }

	// Use this for initialization
	void Start () 
    {
        this.transform.localScale = new Vector3(ForestController.FOREST_X, 10, ForestController.FOREST_Y) / 5;
        Bounds emissionBounds = new Bounds();
        emissionBounds.center = new Vector3(0, -0.5f, 0);
        emissionBounds.min = new Vector3(-ForestController.FOREST_X, 0, -ForestController.FOREST_Y);
        emissionBounds.max = new Vector3(ForestController.FOREST_X, 0, ForestController.FOREST_Y);

        soilWater = this.GetComponent<Emitter>();
        soilWater.SpatialExtent = emissionBounds;
        GetComponent<BoxCollider>().size = new Vector3(ForestController.FOREST_X, 1, ForestController.FOREST_Y);
       
        soilWater.EmissionForce = 0;
        soilWater.EmissionRate = 0.5f;
        //soilWater.StartEmitter();

        stand = new List<Tree>();

        for (int i = 0; i < STAND_SIZE / 4; i++)
        {
            Vector3 spot = new Vector3(Random.Range(-ForestController.FOREST_X, ForestController.FOREST_X), 0, Random.Range(-ForestController.FOREST_Y, ForestController.FOREST_Y));
            PlacePlant(spot);
        }

        StartCoroutine("Succession");
	}

    // Update is called once per frame
    void Update() 
    {
		
	}

    public void PlacePlant(Vector3 spot, bool local = false)
    {
        GameObject sapling = GameObject.Instantiate(tree);
        sapling.transform.SetParent(this.gameObject.transform, false);
        if(local)
            sapling.transform.localPosition = spot;
        else
            sapling.transform.position = spot;

        stand.Add(sapling.GetComponent<Tree>());
    }

    /*void OnEnable()
    {
        //StartCoroutine("Succession");
        foreach (Emitter em in GetComponentsInChildren<Emitter>())
        {
            em.StartExchanger();
        }
    }

    void OnDisable()
    {
        //StopCoroutine("Succession");
        foreach (Emitter em in GetComponentsInChildren<Emitter>())
        {
            em.StopExchanger();
        }
    }*/

    public void RemoveTree(Tree tree)
    {
        stand.Remove(tree);
    }

    IEnumerator Succession()
    {
        while (true)
        {
            yield return new WaitForSeconds(SUCCESSION_RATE);
            if (stand.Count < STAND_SIZE)
            {
                Vector3 spot = new Vector3(Random.Range(-ForestController.FOREST_X, ForestController.FOREST_X), 0, Random.Range(-ForestController.FOREST_Y, ForestController.FOREST_Y))/2;
                PlacePlant(spot, true);
            }
        }
    }
}
