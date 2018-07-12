using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour 
{
    GameObject stem, crown, soilWater;
    Material stemMat;
    public Color stemBrown;
    private float stemWidth, stemHeight, waterAvailability;

    public const float WATER_SCALING = 2f, WATER_MIN = 0, WATER_MAX = 1;
    public float WaterAvailability 
    {
        get { return waterAvailability; }
        set 
        {
            waterAvailability = Mathf.Clamp(value, WATER_MIN, WATER_MAX);
            soilWater.transform.localScale = new Vector3(waterAvailability, 0.1f, waterAvailability) * WATER_SCALING;
        }
    }

    public const float JUVENILE_MIN = 0.2f, JUVENILE_MAX = 0.25f;
    public const float MORTALITY = 1f, MORTALITY_RATE = 0.05f, SIZE_MORTALITY = 0f;
    private float deathLerp, deathWidth;
    public bool Dying = false;

    public const float CROWN_SCALING = 4f;
    public float StemWidth 
    { 
        get { return stemWidth; } 
        set 
        { 
            stemWidth = value;
            StemHeight = stemWidth * HEIGHT_SCALING; 
            GetComponentInChildren<Crown>().CrownRadius = (stemWidth * CROWN_SCALING);
        } 
    }

    public const float HEIGHT_SCALING = 4f;
    public float StemHeight
    {
        get { return stemHeight; }
        set
        {
            stemHeight = value; 
            stem.transform.localPosition = new Vector3(0, stemHeight, 0);
            stem.transform.localScale = new Vector3(stemWidth, stemHeight, stemWidth);
        }
    }

    void Awake()
    {
        stem = this.transform.GetChild(0).gameObject;
        stemMat = stem.GetComponent<MeshRenderer>().material;
        stemBrown = stemMat.color;
        
        crown = this.transform.GetChild(1).gameObject;
        soilWater = this.transform.GetChild(2).gameObject;
    }

    // Use this for initialization
	void Start () 
    {
        StemWidth = Random.Range(JUVENILE_MIN, JUVENILE_MAX);
        WaterAvailability = 0.75f;
        //respirator.StartExchanger();
	}
	
	// Update is called once per frame
    void Update()
    {
        //Probability of mortality based on size and water
        if (stemWidth >= (MORTALITY - (stemWidth * SIZE_MORTALITY)) && !Dying)
        {
            Die();
            return;
        }
    }

    public void Die()
    {
        deathWidth = stemWidth;
        StartCoroutine("Mortality");
        Dying = true;
    }

    IEnumerator Mortality()
    {
        deathLerp = 0f;
        while (true)
        {
            yield return new WaitForSeconds(MORTALITY_RATE);
            StemWidth = Mathf.Lerp(deathWidth, 0f, deathLerp);
            crown.GetComponent<Crown>().CrownDeath(deathLerp);
            if (deathLerp >= 1f)
            {
                StopCoroutine("Mortality");
                foreach (Transform tt in transform.GetComponentsInChildren<Transform>())
                    GameObject.Destroy(tt.gameObject);
                SendMessageUpwards("RemoveTree", this);
                GameObject.Destroy(this);
                yield break;
            }
            deathLerp += Time.deltaTime;
        }
    }
}
