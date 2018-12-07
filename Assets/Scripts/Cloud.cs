using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour 
{
    PerlinSphere nimbus;
    public Sky CloudSky;

    public Mesh CloudMesh { get { return this.GetComponent<MeshFilter>().sharedMesh; } }
    public const int CLOUD_MESH_LONGITUDE = 48, CLOUD_MESH_LATITUDE = 32;

    //Iterated for rain and vapor content
    public const float CLOUD_PERLIN_SHIFT_MIN = 0.01f, CLOUD_PERLIN_SHIFT_MAX = 0.1f;
    public const float CLOUD_PERLIN_RADIUS_MIN = 1f, CLOUD_PERLIN_RADIUS_MAX = 3f;
    //Randomly varied for visual effect
    public const float CLOUD_PERLIN_INTER_MIN = 0.01f, CLOUD_PERLIN_INTER_MAX = 0.05f;

    public const float CLOUD_OUTLINE_WIDTH_MIN = 0.002f, CLOUD_OUTLINE_WIDTH_MAX = 0.03f;
  
    public float PrecipitationRate = 3f;
    private float lastPrecip = 0;

    public bool Raining = false;
    public const float RAIN_RATE = 0.05f;
    public const int DEW_POINT = 25, CONDENSATION_RATE = 5;
    public int Vapor = 0;

    private GameObject rain;

    void Awake()
    {
        this.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        nimbus = this.GetComponent<PerlinSphere>();
        rain = Resources.Load("Prefabs/Rain") as GameObject;
    }

	// Use this for initialization
	void Start () 
    {
        nimbus.PerlinInterval = Random.Range(CLOUD_PERLIN_INTER_MIN, CLOUD_PERLIN_INTER_MAX);
        nimbus.Radius = CLOUD_PERLIN_RADIUS_MIN;
        nimbus.PerlinShift = CLOUD_PERLIN_SHIFT_MIN;

        GetComponent<CapsuleCollider>().center = Vector3.zero;
        GetComponent<CapsuleCollider>().height = nimbus.Radius;
        GetComponent<CapsuleCollider>().radius = nimbus.Radius;
	}
	
	// Update is called once per frame
	void Update () 
    {
        nimbus.MakeCloud(CloudMesh);
        if (Time.time - lastPrecip > PrecipitationRate)
        {
            lastPrecip = Time.time;
            Vapor += CONDENSATION_RATE;
            VaporRoutine();
        }
	}

    void VaporRoutine()
    {
        if (Vapor > DEW_POINT)
        {
            StartCoroutine("Rain");
        }
        else if (Vapor <= 0)
        {
            CloudSky.RemoveCloud(this);
            StopAllCoroutines();
            GameObject.Destroy(this.gameObject);
        }

        float dewLerp = Vapor / (float)DEW_POINT;
        this.GetComponent<MeshRenderer>().material.SetFloat("_Outline", Mathf.Lerp(CLOUD_OUTLINE_WIDTH_MIN, CLOUD_OUTLINE_WIDTH_MAX, dewLerp));

        nimbus.Radius = Mathf.Lerp(CLOUD_PERLIN_RADIUS_MIN, CLOUD_PERLIN_RADIUS_MAX, dewLerp);
        nimbus.PerlinShift = Mathf.Lerp(CLOUD_PERLIN_SHIFT_MIN, CLOUD_PERLIN_SHIFT_MAX, dewLerp);

        nimbus.MakeCloud(CloudMesh);
        GetComponent<CapsuleCollider>().center = Vector3.zero;
        GetComponent<CapsuleCollider>().radius = nimbus.Radius * 2;
        GetComponent<CapsuleCollider>().height = nimbus.Radius * 2;
    }

    public void OnTriggerEnter(Collider other)
    {
        FluxRibbon fr = other.GetComponentInParent<FluxRibbon>();
        if (fr != null)
        {
            if (fr.Fluxer != GetComponent<Emitter>() && !Raining)
            {
                Vapor++;
                VaporRoutine();
            }
            fr.Fluxed();
        }
    }

    //Use Ray generation from solar object to make (it) rain
    IEnumerator Rain()
    {
        while (true)
        {
            yield return new WaitForSeconds(RAIN_RATE);
            Vapor--;
            VaporRoutine();
            AddRain();
        }
    }

    void AddRain()
    {
        GameObject newRay = GameObject.Instantiate(rain) as GameObject;

        newRay.transform.SetParent(FindObjectOfType<EcosystemController>().transform, false);
        newRay.transform.localRotation = Quaternion.identity;
        newRay.transform.Rotate(180, 0, 0);

        Vector2 offset = Random.insideUnitCircle;
        offset = new Vector2(offset.x * nimbus.Radius, offset.y * nimbus.Radius);
        newRay.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y, this.transform.position.z + offset.y);
        newRay.transform.localScale = new Vector3(RainDrop.RAIN_WIDTH, RainDrop.RAIN_LENGTH, RainDrop.RAIN_WIDTH);

        RainDrop drop = newRay.GetComponent<RainDrop>();
        
        drop.origScale = newRay.transform.localScale;
        drop.origPos = newRay.transform.localPosition;
        drop.goalPos = drop.origPos + (Vector3.down * this.transform.localPosition.y);
        drop.goalScale = new Vector3(0, this.transform.localPosition.y * RainDrop.RAIN_LENGTH, 0);
        drop.Nimbus = this;
    }
}
