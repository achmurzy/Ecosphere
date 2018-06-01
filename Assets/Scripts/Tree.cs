using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour 
{
    GameObject stem, crown;
    public float stemWidth, stemHeight, crownRadius;

    void Awake()
    {
        stem = this.gameObject;
        crown = this.transform.GetChild(0).gameObject;
        stem.transform.localScale = new Vector3(stemWidth, stemHeight, stemWidth);
        crown.transform.localScale = Vector3.one * crownRadius;
    }

    // Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}
}
