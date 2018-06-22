using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stand : MonoBehaviour 
{
    GameObject tree;
    List<Tree> stand;

    void Awake()
    {
        tree = Resources.Load("Prefabs/Tree") as GameObject;
    }

	// Use this for initialization
	void Start () 
    {
        stand = new List<Tree>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Succession()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (stand.Count < 20)
            {
                GameObject sapling = GameObject.Instantiate(tree);
                sapling.transform.SetParent(this.gameObject.transform, false);
                sapling.transform.localPosition = new Vector3(Random.RandomRange(-ForestController.FOREST_X, ForestController.FOREST_X), 0, Random.RandomRange(-ForestController.FOREST_Y, ForestController.FOREST_Y));
                stand.Add(sapling.GetComponent<Tree>());
            }
        }
    }

    void OnEnable()
    {
        StartCoroutine("Succession");
        foreach (Emitter em in GetComponentsInChildren<Emitter>())
        {
            em.StartExchanger();
        }
    }

    void OnDisable()
    {
        StopCoroutine("Succession");
        foreach (Emitter em in GetComponentsInChildren<Emitter>())
        {
            em.StopExchanger();
        }
    }

    public void RemoveTree(Tree tree)
    {
        stand.Remove(tree);
    }
}
