using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

//The Ocean needs to be an Aqua cube. Make it
public class OceanController : MonoBehaviour {

    Vector3 oceanExtent = new Vector3(10, 10, 10);
    List<Coral> reef;
    GameObject sol;
    GameObject oceanSurface, seawater;

    void Awake()
    {
        reef = new List<Coral>();
        sol = GameObject.Find("Sol");
        oceanSurface = GameObject.Find("Water");
        seawater = GameObject.Find("Seawater");
    }

    // Use this for initialization
    void Start()
    {
        seawater.transform.localScale = oceanExtent;
        seawater.transform.localPosition = new Vector3(0, oceanExtent.y / 2, 0);
        
        oceanSurface.GetComponent<Emitter>().SpatialCenter = new Vector3(0, 0, 0);
        oceanSurface.GetComponent<Emitter>().SpatialExtent = new Vector3(oceanExtent.x, 0, oceanExtent.y);
        oceanSurface.GetComponent<Emitter>().DestructionTrigger.size = new Vector3(oceanExtent.x, 1, oceanExtent.y);
        oceanSurface.GetComponent<Emitter>().EmissionForce = 0;
        //oceanSurface.GetComponent<Emitter>().StartEmitter();

        StartCoroutine("Succession");
    }

    // Update is called once per frame
    void Update()
    { }

    IEnumerator Succession()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (reef.Count < 10)
            {
                GameObject tree = GameObject.Instantiate(Resources.Load("Prefabs/Coral")) as GameObject;
                tree.transform.parent = this.gameObject.transform;
                tree.transform.localPosition = new Vector3(Random.RandomRange(-oceanExtent.x, oceanExtent.x), -oceanExtent.y, Random.RandomRange(-oceanExtent.y, oceanExtent.y));
                reef.Add(tree.GetComponent<Coral>());
            }
        }
    }

    public void RemoveCoral(Coral coral)
    {
        reef.Remove(coral);
    }
}
