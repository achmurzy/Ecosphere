using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ocean : MonoBehaviour {

    public Slider HeatSlider, AcidSlider;   //CO2 is really driving these. Can't we just use that?
    MeshCollider oceanCollider;
    OceanController oceanController;

    public float oceanSize = 1;
    public float OceanSize { get { return oceanSize * 5; } private set { oceanSize = value; } }

    public GameObject Seawater;
    Material seawaterMat;
    Color seawaterColor, acidColor;

    void Awake()
    {
        oceanController = GetComponentInParent<OceanController>();

        seawaterMat = Material.Instantiate(Resources.Load("Materials/Seawater") as Material);
        seawaterColor = seawaterMat.color;
        acidColor = seawaterColor;
        acidColor.b = 85f;
    }

	// Use this for initialization
	void Start () 
    {
        Seawater = GameObject.Find("Seawater");	
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}

    public void HeatOcean()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.red, HeatSlider.value);
    }

    public void Acidify()
    {
        Seawater.GetComponent<MeshRenderer>().material.color = Color.Lerp(seawaterColor, acidColor, AcidSlider.value);
    }

    public void OnCollisionEnter(Collision collision)
    {
        float prob = HeatSlider.value;
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
