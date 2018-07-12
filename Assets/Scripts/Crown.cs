using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : MonoBehaviour, IPhotosensitive, ITouchable, IEmitter
{
    Tree wholePlant; 
    Material luxMat, crownMat;
    private const float CROWN_FLASH = 0.01f, H2O_SCALE = 0.1f, EMISSION_SCALING = 1f, TRIGGER_SCALING = 2f;
    private const float GROWTH_ITER = 0.05f, WATER_USE_FACTOR = 4f;
    private float startWidth, startWater;
    private float crownFlashLerp = 0f, crownFlashBuffer = 1f, crownFlashCounter = 0f;
    Color crownGreen, luxYellow, luxOutline;

    Emitter transpirator;
    public const float WATER_FLUX_RATE = 5.0f, WATER_FLUX_FORCE = 0.05f;

    PerlinSphere crownSphere;
    private const float CROWN_PERLIN_INTERVAL_MIN = 0f, CROWN_PERLIN_INTERVAL_MAX = 0.5f;
    private float crownRadius;
    public float CrownRadius
    {
        get { return crownRadius; }
        set
        {
            crownRadius = value;

            transform.localScale = Vector3.one * crownRadius;
            transform.localPosition = new Vector3(0, 2 * GetComponentInParent<Tree>().StemHeight, 0);
            transpirator.SpatialExtent = new Bounds();
            transpirator.SpatialExtent.center = Vector3.zero;
            transpirator.SpatialExtent.min = Vector3.zero;
            transpirator.SpatialExtent.max = Vector3.one * crownRadius;

            //GetComponent<SphereCollider>().radius = TRIGGER_SCALING;
            
            transpirator.LowerEmissionTrajectory = new Vector3(-1, 0, -1);
            transpirator.UpperEmissionTrajectory = Vector3.one;
            transpirator.EmissionRate = WATER_FLUX_RATE / crownRadius * EMISSION_SCALING;
 
            transpirator.EmissionForce = WATER_FLUX_FORCE;
        }
    }

    void Awake()
    {
        transpirator = GetComponentInChildren<Emitter>();

        crownSphere = GetComponent<PerlinSphere>();
        GetComponent<MeshFilter>().mesh = crownSphere.MakeCrown();

        crownMat = GetComponent<MeshRenderer>().material;
        luxMat = Material.Instantiate(Resources.Load("Materials/Lux") as Material);
        luxYellow = luxMat.color;
        luxOutline = luxMat.GetColor("_OutlineColor");
        crownGreen = crownMat.color;

        wholePlant = GetComponentInParent<Tree>();
    }

	// Use this for initialization
	void Start () 
    {
        transpirator.StartEmitter();
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public void TriggerEnter(Emitter.EmitterPackage ep)
    {

    }

    public void TriggerExit(Emitter.EmitterPackage ep)
    {
        FluxRibbon fr = ep.flux;
        Collider other = ep.other;

        Cloud myCloud = null;
        float lowDist = Mathf.Infinity;
        Sky mySky = FindObjectOfType<Sky>();
        
        foreach (Cloud cc in mySky.Firmament)
        {
            float cloudDist = Vector3.Distance(fr.transform.position, cc.transform.position);
            if (cloudDist  < lowDist)
            {
                myCloud = cc;
                lowDist = cloudDist;
            }
        }

        if (myCloud != null)
        {
            fr.Lifespan--;
            fr.transform.LookAt(myCloud.transform.position);
        }
    }

    public void Rainfall()
    {
        wholePlant.WaterAvailability += (GROWTH_ITER*WATER_USE_FACTOR);
    }

    public string Touch()
    {
        return "Tree";
    }

    public bool LightEnter(SolarRay ray)
    {
        if (Time.time - crownFlashCounter > crownFlashBuffer && crownFlashLerp == 0f && !wholePlant.Dying)
        {
            startWater = wholePlant.WaterAvailability;
            if (startWater < startWidth)
                wholePlant.Die();
            else
            {
                GetComponent<MeshRenderer>().material = luxMat;
                startWidth = wholePlant.StemWidth;
                StartCoroutine("CrownFlash");
            }
            return true;
        }
        return false;
    }

    IEnumerator CrownFlash()
    {
        while (true)
        {
            yield return new WaitForSeconds(CROWN_FLASH);
            luxMat.color = Color.Lerp(luxYellow, crownGreen, crownFlashLerp);
            luxMat.SetColor("_OutlineColor", Color.Lerp(Color.white, luxOutline, crownFlashLerp));

            wholePlant.StemWidth = Mathf.Lerp(startWidth, startWidth + GROWTH_ITER, crownFlashLerp);
            wholePlant.WaterAvailability = Mathf.Lerp(startWater, startWater - (WATER_USE_FACTOR*GROWTH_ITER), crownFlashLerp);
            
            if (crownFlashLerp >= 1f)
            {
                EndFlash();
                yield break;
            }
            crownFlashLerp += Time.deltaTime;
        }
    }

    public void EndFlash()
    {
        luxMat.color = Color.yellow;
        GetComponent<MeshRenderer>().material = crownMat;
        crownFlashCounter = Time.time;
        crownFlashLerp = 0f;
        StopCoroutine("CrownFlash");
    }

    public void CrownDeath(float deathLerp)
    {
        GetComponent<MeshRenderer>().material.color = Color.Lerp(crownGreen, wholePlant.stemBrown, deathLerp);
        crownSphere.PerlinInterval = Mathf.Lerp(CROWN_PERLIN_INTERVAL_MIN, CROWN_PERLIN_INTERVAL_MAX, deathLerp);
        GetComponent<MeshFilter>().mesh = crownSphere.MakeCrown();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {
        //StopCoroutine("CrownFlash");
    }
}
