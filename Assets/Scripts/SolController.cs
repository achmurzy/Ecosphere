using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolController : MonoBehaviour 
{
    public Slider IntensitySlider;
    private GameObject Pivot, Light;
    GameObject Beam;
    public float RadiationIntensity, SolarPeriod = 0f;
    
    public float SolDistance = 50;
    public const float EXTENT_SCALING = 1f;

    void Awake()
    {
        Pivot = transform.parent.gameObject;
        transform.localPosition = Pivot.transform.up * SolDistance;
        Beam = Resources.Load("Prefabs/Ray") as GameObject;
        GetComponentInChildren<Light>().transform.rotation = Quaternion.identity; 
    }
    
    void Start () 
    {
        Sunrise();
	}

	// Update is called once per frame
	void Update () 
    {
        //Pivot.transform.Rotate(Pivot.transform.forward * SolarPeriod);
        transform.LookAt(Vector3.zero);
        if (transform.position.y > 0)
        {
            //Sunrise();
        }
        else
        {
            //Sunset();
        }
	}

    void AddRay()
    {
        GameObject newRay = GameObject.Instantiate(Beam, this.transform) as GameObject;
        newRay.GetComponent<SolarRay>().Sol = this;

        newRay.transform.LookAt(Vector3.zero);
        newRay.transform.Rotate(90, 0, 0);
        Vector2 offset = Random.insideUnitCircle * Vector3.Magnitude(this.transform.localScale) * EXTENT_SCALING;
        newRay.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y, this.transform.position.z + offset.y);
        newRay.transform.localScale = new Vector3(SolarRay.RAY_WIDTH, SolarRay.RAY_WIDTH, SolarRay.RAY_WIDTH) * RadiationIntensity;
    }

    IEnumerator Shine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 - RadiationIntensity);
            AddRay();
        }
    }

    public void Sunrise()
    {
        StartCoroutine("Shine");
    }
    public void Sunset()
    {
        StopCoroutine("Shine");
    }
    public void AttenuateLight()
    {
        RadiationIntensity = IntensitySlider.value;
    }

    void OnEnable()
    {
        Sunrise();
    }

    void OnDisable()
    {
        Sunset();
    }
}
