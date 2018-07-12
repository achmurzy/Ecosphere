using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour, IPhotosensitive, ITouchable, IEmitter
{
    GameObject water;
    public PerlinSphere bush;
    Emitter latentHeatFluxer;

    Material plantMat, dessicateMat, luxMat;
    Color greenColor, brownColor, luxColor, luxOutline; 

    public Mesh PlantMesh { get { return this.GetComponent<MeshFilter>().mesh; } set { this.GetComponent<MeshFilter>().mesh = value; } }

    private const float PLANT_FLASH = 0.01f;
    private float plantFlashLerp, plantFlashCounter;

    public const float HEAT_FLUX_RATE_MIN = 4.0f, HEAT_FLUX_RATE_MAX = 1.0f, HEAT_FLUX_FORCE = 0.1f;
    public const float PLANT_PERLIN_RADIUS_MIN = 2f, PLANT_PERLIN_RADIUS_MAX = 4f;
    public const float PLANT_PERLIN_SHIFT_MIN = 0.01f, PLANT_PERLIN_SHIFT_MAX = 0.05f;
    public const float PLANT_PERLIN_INTER_MIN = 0.01f, PLANT_PERLIN_INTER_MAX = 1.0f;

    public string Touch() { return "Crops"; }

    public const float GROWTH_RATE = 1f, YIELD_QUANTUM = 0.01f;
    private float yield = 0.25f;
    public float Yield { get { return yield; } set { yield = value; } }
    public const float YIELD_MORTALITY = 0.1f;

    private bool photoInhibit = false;
    private const float SOLAR_DROUGHT = 10f;

    void Awake()
    {
        latentHeatFluxer = this.GetComponent<Emitter>();
        bush = this.GetComponent<PerlinSphere>();

        plantMat = this.GetComponent<MeshRenderer>().material;
        greenColor = plantMat.color;
        dessicateMat = Material.Instantiate(Resources.Load("Materials/Outline/Brown") as Material);
        brownColor = dessicateMat.color;
        luxMat = Material.Instantiate(Resources.Load("Materials/Lux") as Material);
        luxColor = luxMat.color;
        luxOutline = luxMat.GetColor("_OutlineColor");

        bush.Radius = Random.Range(PLANT_PERLIN_RADIUS_MIN, PLANT_PERLIN_RADIUS_MAX);
        bush.PerlinInterval = Random.Range(PLANT_PERLIN_INTER_MIN, PLANT_PERLIN_INTER_MAX);
        bush.PerlinShift = Random.Range(PLANT_PERLIN_SHIFT_MIN, PLANT_PERLIN_SHIFT_MAX);
        PlantMesh = bush.MakePlant();
    }

	// Use this for initialization
	void Start () 
    {
        Bounds emissionBounds = new Bounds();
        emissionBounds.min = Vector3.zero;
        emissionBounds.max = Vector3.up;
        emissionBounds.center = Vector3.up;

        latentHeatFluxer.LowerEmissionTrajectory = new Vector3(0, 0, 0);
        latentHeatFluxer.UpperEmissionTrajectory = new Vector3(0.1f, 1, 0.1f);
        latentHeatFluxer.SpatialExtent = emissionBounds;

        BoxCollider bc = GetComponent<BoxCollider>();
        bc.center = Vector3.up * bush.Radius;
        bc.size = new Vector3(10, 20, 10);
        
        latentHeatFluxer.EmissionForce = HEAT_FLUX_FORCE;
        latentHeatFluxer.EmissionRate = HEAT_FLUX_RATE_MIN;
        latentHeatFluxer.StartEmitter();

        latentHeatFluxer.EmissionRate = Mathf.Lerp(HEAT_FLUX_RATE_MIN, HEAT_FLUX_RATE_MAX, Yield);
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, yield);
        StartCoroutine("Growth");
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}

    public IEnumerator Growth()
    {
        while (true)
        {
            yield return new WaitForSeconds(GROWTH_RATE);

            if (photoInhibit)
            {
                if (water != null)
                {
                    StartCoroutine("Drought");
                }
                else
                    yield -= YIELD_QUANTUM * SOLAR_DROUGHT;
                photoInhibit = false;
            }
            else
            {
                if (water == null)
                {
                    yield -= YIELD_QUANTUM * (SOLAR_DROUGHT/2);
                }
                else
                    yield += YIELD_QUANTUM;
                
            }
            if (YIELD_MORTALITY > yield)
                GameObject.Destroy(this.gameObject);

            latentHeatFluxer.EmissionRate = Mathf.Lerp(HEAT_FLUX_RATE_MIN, HEAT_FLUX_RATE_MAX, Yield);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, yield);
        }
    }

    public IEnumerator Drought()
    {
        float droughtLerp = 0f;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            water.transform.localScale = Vector3.Lerp(new Vector3(bush.Radius, 0.1f, bush.Radius), Vector3.zero, droughtLerp);
            this.GetComponent<MeshRenderer>().material.color = Color.Lerp(greenColor, brownColor, droughtLerp);
            if (droughtLerp >= 0.99f)
            {
                GameObject.Destroy(water);
                StopCoroutine("Drought");
                yield break;
            }
            droughtLerp += Time.deltaTime;
        }
    }

    IEnumerator PlantFlash()
    {
        while (true)
        {
            yield return new WaitForSeconds(PLANT_FLASH);
            luxMat.color = Color.Lerp(luxColor, plantMat.color, plantFlashLerp);
            luxMat.SetColor("_OutlineColor", Color.Lerp(Color.white, luxOutline, plantFlashLerp));

            if (plantFlashLerp >= 1f)
            {
                luxMat.color = Color.yellow;
                GetComponent<MeshRenderer>().material = plantMat;
                plantFlashCounter = Time.time;
                plantFlashLerp = 0f;
                photoInhibit = true;
                StopCoroutine("PlantFlash");
                yield break;
            }
            plantFlashLerp += Time.deltaTime;
        }
    }

    //Here, we want to shade the plants. If light enters here, the plant will suffer (and die)
    //What is drought? Browning and reduction in size to death. Yield is the parameter for this.
    public bool LightEnter(SolarRay ray)
    {
        //PlantMesh = bush.MakePlant(); show dessication with this method
        GetComponent<MeshRenderer>().material = luxMat;
        StartCoroutine("PlantFlash");
        return true;
    }

    public void TriggerEnter(Emitter.EmitterPackage ep)
    {}

    public void TriggerExit(Emitter.EmitterPackage ep)
    {
        FluxRibbon fr = ep.flux;
        Collider other = ep.other;
        if (other.gameObject == this.gameObject)
        {
            fr.Fluxed();
        }
    }

    public void SetWater(GameObject sWater)
    {
        water = sWater;
        sWater.transform.position = this.transform.position;
        sWater.transform.localScale = new Vector3(bush.Radius, 0.1f, bush.Radius);
    }

    void OnEnable()
    {
        StartCoroutine("Growth");
    }

    void OnDisable()
    {
        StopCoroutine("Growth");
    }

    void OnDestroy()
    {
        SendMessageUpwards("RemovePlant", this, SendMessageOptions.DontRequireReceiver);
    }
}
