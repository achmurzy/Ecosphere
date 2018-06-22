using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ocean : MonoBehaviour {

    MeshCollider oceanCollider;
    OceanController oceanController;

    public float oceanSize = 1;
    public float OceanSize { get { return oceanSize * 5; } private set { oceanSize = value; } }

    public GameObject Seawater;
    Material seawaterMat;
    Color seawaterColor, acidColor;

    public Slider HeatSlider, AcidSlider;
    public float OceanSurfaceTemperature { get; set; }
    public float OceanAcidity { get; set; }
    public const float PARTICLE_ACIDIDTY = 0.01f;

    void Awake()
    {
        oceanController = GetComponentInParent<OceanController>();
    }

	// Use this for initialization
	void Start () 
    {
        Seawater = GameObject.Find("Seawater");

        Seawater.transform.localScale = new Vector3(2*OceanController.OCEAN_X, 2*OceanController.OCEAN_Y, 2*OceanController.OCEAN_Z);
        Seawater.transform.localPosition = new Vector3(0, OceanController.OCEAN_Y, 0);

        this.transform.localScale = new Vector3((1f / 5f) * OceanController.OCEAN_X, OceanController.OCEAN_Y, (1f / 5f) * OceanController.OCEAN_Z);
        this.transform.localPosition = new Vector3(0, (OceanController.OCEAN_Y * 2) + 0.01f, 0);
        Bounds emissionBounds = new Bounds();
        emissionBounds.center = new Vector3(0, -5, 0);
        emissionBounds.min = new Vector3(-OceanController.OCEAN_X, 0, -OceanController.OCEAN_Z);
        emissionBounds.max = new Vector3(OceanController.OCEAN_X, 0, OceanController.OCEAN_Z);
        this.GetComponent<Emitter>().SpatialExtent = emissionBounds;
        
        this.GetComponent<Emitter>().DestructionTrigger.size = new Vector3(OceanController.OCEAN_X, 5, OceanController.OCEAN_Z);
        this.GetComponent<Emitter>().EmissionForce = 0;
        this.GetComponent<Emitter>().StartExchanger();

        seawaterMat = Material.Instantiate(Resources.Load("Materials/Seawater") as Material);
        seawaterColor = seawaterMat.color;
        acidColor = seawaterColor;
        acidColor.b = 85f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}

    public void HeatOcean()
    {
        OceanSurfaceTemperature = HeatSlider.value;
        LerpOceanColor(OceanSurfaceTemperature);
        //BroadcastMessage("StressCoral", HeatSlider.value);
    }

    public void AcidifyOcean()
    {
        OceanAcidity += PARTICLE_ACIDIDTY;
        LerpSeawaterColor(OceanAcidity);
    }

    public void LerpOceanColor(float heatValue)
    {
        this.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.red, heatValue);
    }

    public void LerpSeawaterColor(float acidValue)
    {
        Seawater.GetComponent<MeshRenderer>().material.color = Color.Lerp(seawaterColor, acidColor, acidValue);
    }

    public void OnCollisionEnter(Collision collision)
    {
        float prob = OceanAcidity;
        float val = Random.RandomRange(0f, 1f);
        if (val > prob)
        {
            //Let the molecule pass by making it a trigger
            collision.collider.isTrigger = true;
        }
        else
        {
            //collision.collider.attachedRigidbody.AddForce((-2)*collision.collider.attachedRigidbody.velocity);
        }
    }
}
