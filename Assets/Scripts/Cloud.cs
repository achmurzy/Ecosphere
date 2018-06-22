using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour 
{
    PerlinSphere nimbus;

    public Mesh CloudMesh { get { return this.GetComponent<MeshFilter>().mesh; } set { this.GetComponent<MeshFilter>().mesh = value; } }
    public const int CLOUD_MESH_LONGITUDE = 48, CLOUD_MESH_LATITUDE = 32;

    //Iterated for rain and vapor content
    public const float CLOUD_PERLIN_SHIFT_MIN = 0.01f, CLOUD_PERLIN_SHIFT_MAX = 0.1f;
    
    //Randomly varied for visual effect
    public const float CLOUD_PERLIN_INTER_MIN = 0.01f, CLOUD_PERLIN_INTER_MAX = 0.05f;

  
    public const float VAPOR_SCALE = 0.5f;
    //10 molecules lead to precipitation
    public const int DEW_POINT = 10;
    public int Vapor = 0;

    void Awake()
    {
        nimbus = this.GetComponent<PerlinSphere>();
    }

	// Use this for initialization
	void Start () 
    {
        nimbus.PerlinInterval = Random.RandomRange(CLOUD_PERLIN_INTER_MIN, CLOUD_PERLIN_INTER_MAX);
        nimbus.PerlinShift = Random.RandomRange(CLOUD_PERLIN_SHIFT_MIN, CLOUD_PERLIN_SHIFT_MAX);
	}
	
	// Update is called once per frame
	void Update () 
    {
        CloudMesh = nimbus.MakeCloud();
	}

    public void AddVapor()
    {
        Vapor++;
        if (Vapor > DEW_POINT)
        {
            Rain();
        }
        else
        {
            nimbus.Radius += VAPOR_SCALE;
            this.GetComponent<MeshFilter>().mesh = nimbus.MakeCloud();
        }
    }

    //Use Ray generation from solar object to make (it) rain
    public void Rain()
    {

    }
}
