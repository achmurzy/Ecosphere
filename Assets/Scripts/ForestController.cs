using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestController : EcosystemController 
{
    Vector2 forestExtent = new Vector2(10, 10);
    List<Tree> forest;
    GameObject sol;
    GameObject soilPlane;

    void Awake()
    {
        forest = new List<Tree>();
        sol = GameObject.Find("Sol");
        soilPlane = GameObject.Find("Soil");
    }

	// Use this for initialization
	void Start () 
    {
        soilPlane.transform.localScale = new Vector3(forestExtent.x, 1, forestExtent.y)/5;
        soilPlane.GetComponent<Emitter>().SpatialCenter = new Vector3(0, -0.5f, 0);
        soilPlane.GetComponent<Emitter>().SpatialExtent = new Vector3(forestExtent.x, 0, forestExtent.y);
        soilPlane.GetComponent<Emitter>().DestructionTrigger.size = new Vector3(forestExtent.x, 1, forestExtent.y);
        soilPlane.GetComponent<Emitter>().EmissionForce = 0;
        soilPlane.GetComponent<Emitter>().StartEmitter();

        StartCoroutine("Succession");
    }
	
	// Update is called once per frame
	void Update () 
    {}

    IEnumerator Succession()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (forest.Count < 10)
            {
                GameObject tree = GameObject.Instantiate(Resources.Load("Prefabs/Tree")) as GameObject;
                tree.transform.parent = this.gameObject.transform;
                tree.transform.localPosition = new Vector3(Random.RandomRange(-forestExtent.x, forestExtent.x), 0, Random.RandomRange(-forestExtent.y, forestExtent.y));
                forest.Add(tree.GetComponent<Tree>());
            }
        }
    }

    public void RemoveTree(Tree tree)
    {
        forest.Remove(tree);
    }
}
