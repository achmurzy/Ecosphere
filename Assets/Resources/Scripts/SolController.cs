using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolController : MonoBehaviour 
{
    GameObject Sunflower, Ray;
    float rayEmission = 0.1f;

	// Use this for initialization
	void Start () 
    {
        Sunflower = GameObject.Find("Sunflower");
        Ray = Resources.Load("Prefabs/Ray") as GameObject;
	}

    IEnumerator Shine()
    {
        while (true)
        {
            yield return new WaitForSeconds(rayEmission);
            AddRay();
        }
    }

	// Update is called once per frame
	void Update () 
    {

	}

    void AddRay()
    {
        GameObject newRay = GameObject.Instantiate(Ray, this.transform) as GameObject;
    }

    public void Sunrise()
    {
        StartCoroutine("Shine");
    }
    public void Sunset()
    {
        StopCoroutine("Shine");
    }
}
