using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour 
{
    GameObject stem, crown;
    private float stemWidth, stemHeight;
    Emitter respirator;
    public const float HEIGHT_SCALING = 5f, CROWN_SCALING = 5f, CO2_SCALING = 0.1f;
    public const float MORTALITY = 2f, EMISSION_SCALING = 1f, TRIGGER_SCALING = 2f;
    public float StemWidth 
    { 
        get { return stemWidth; } 
        set 
        { 
            stemWidth = value;
            if (stemWidth >= MORTALITY)
            {
                foreach (Transform tt in transform.GetComponentsInChildren<Transform>())
                    GameObject.Destroy(tt.gameObject);
                SendMessageUpwards("RemoveTree", this);
                GameObject.Destroy(this);
                return;
            }
            StemHeight = stemWidth * HEIGHT_SCALING; 
            GetComponentInChildren<Crown>().CrownRadius = stemWidth * CROWN_SCALING;
            Bounds emissionBounds = new Bounds();
            emissionBounds.min = Vector3.zero;
            emissionBounds.max = new Vector3(stemWidth, stemHeight, stemWidth);
            respirator.SpatialExtent = emissionBounds;
            
            respirator.DestructionTrigger.size = Vector3.one * TRIGGER_SCALING; //The trigger zone, as a child of the stem, scales with it directly , so we only add a scalar coefficient
            respirator.EmissionRate = 1f / stemHeight * EMISSION_SCALING;
        } 
    }
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
        respirator = stem.GetComponentInChildren<Emitter>();
        respirator.Molecule.GetComponent<Molecule>().MolecularScale = CO2_SCALING;
        
        crown = this.transform.GetChild(1).gameObject;
    }

    // Use this for initialization
	void Start () 
    {
        StemWidth = Random.RandomRange(0.1f, 0.5f);
        respirator.StartExchanger();
	}
	
	// Update is called once per frame
    void Update()
    {

    }
}
