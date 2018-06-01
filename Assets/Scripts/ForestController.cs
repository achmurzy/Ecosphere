using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestController : MonoBehaviour 
{
    Vector2 forestExtent = new Vector2(10, 10);
    List<Tree> forest;

    void Awake()
    {
        forest = new List<Tree>();
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
