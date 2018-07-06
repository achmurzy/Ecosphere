using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour, IPhotosensitive, ITouchable
{
    PerlinSphere bush;
    Emitter latentHeatFluxer;

    public Mesh PlantMesh { get { return this.GetComponent<MeshFilter>().mesh; } set { this.GetComponent<MeshFilter>().mesh = value; } }

    public const float HEAT_FLUX_RATE = 1.9f, HEAT_FLUX_FORCE = 0.1f;
    public const float PLANT_PERLIN_SHIFT_MIN = 0.01f, PLANT_PERLIN_SHIFT_MAX = 0.05f;
    public const float PLANT_PERLIN_INTER_MIN = 0.01f, PLANT_PERLIN_INTER_MAX = 1.0f;

    public string Touch() { return "Crops"; }

    void Awake()
    {
        latentHeatFluxer = this.GetComponent<Emitter>();
        bush = this.GetComponent<PerlinSphere>();

        bush.PerlinInterval = Random.RandomRange(PLANT_PERLIN_INTER_MIN, PLANT_PERLIN_INTER_MAX);
        bush.PerlinShift = Random.RandomRange(PLANT_PERLIN_SHIFT_MIN, PLANT_PERLIN_SHIFT_MAX);
        PlantMesh = bush.MakePlant();
    }

	// Use this for initialization
	void Start () 
    {
        Bounds emissionBounds = new Bounds();
        emissionBounds.min = -this.gameObject.transform.localScale / 2;
        emissionBounds.max = this.gameObject.transform.localScale / 2;
        latentHeatFluxer.LowerEmissionTrajectory = new Vector3(-1, 0, -1);
        latentHeatFluxer.UpperEmissionTrajectory = new Vector3(1, 1, 1);
        latentHeatFluxer.SpatialExtent = emissionBounds;
        
        latentHeatFluxer.DestructionTrigger.size = new Vector3(2, 2, 2);
        latentHeatFluxer.EmissionForce = HEAT_FLUX_FORCE;
        latentHeatFluxer.EmissionRate = HEAT_FLUX_RATE;
        latentHeatFluxer.StartEmitter();
	}
	
	// Update is called once per frame
	void Update () 
    {
        PlantMesh = bush.MakePlant();
	}

    //Here, we want to shade the plants. If light enters here, the plant will suffer (and die)
    public bool LightEnter(SolarRay ray)
    {
        return false;
    }

    void OnEnable()
    {
       //StartCoroutine("
    }

    void OnDisable()
    {

    }
}
