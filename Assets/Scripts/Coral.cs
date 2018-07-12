using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Consider photosensitivity
public class Coral : MonoBehaviour, ITouchable 
{
    private Reef reef;
    private GameObject Branch;
    private bool branched = false, bleached = false;
    public const int MAX_BRANCHES = 5;
    private float bleachTime;

    public const float VERTICAL_GROWTH = 2.5f, BRANCH_PROB = 0.1f, CHILD_PROPORTION = 0.22f;
    public const float CORAL_GROWTH = 0.1f, BLEACH_DEATH = 3f;
    
    //Each coral receives 10 growth iterations.
    private float maxGrowth = 0.15f;
    public float MaxGrowth { get; set; }
    private float growthRate = 0.01f;
    public float GrowthRate { get; set; }

    private int generation = 1;
    public int Generation { get { return generation; } set { generation = value; if (generation > MAX_BRANCHES) branched = true; } }
    public bool RightAxis { get; set; }

    private float coralSize;
    public float CoralSize 
    { 
        get { return coralSize; }
        private set 
        { 
            coralSize = value;
            this.transform.localScale += new Vector3(growthRate, VERTICAL_GROWTH * growthRate, growthRate);
            this.transform.position += this.transform.up * VERTICAL_GROWTH * growthRate;
        }
    }

    public float BranchSensitivity { get { return 1 - (Generation/(float)(MAX_BRANCHES+1)); } }
    private float tempStress;
    public float TemperatureStress { get { return tempStress; } 
        private set 
        { 
            tempStress = value;
            if (!bleached)
            {
                this.GetComponent<MeshRenderer>().material.color = Color.Lerp(coralColor, Color.white, tempStress);
                if (tempStress >= (BranchSensitivity+0.05f))
                {
                    bleached = true;
                    bleachTime = Time.time;
                    StartCoroutine("Evacuation");
                    StopCoroutine("CoralGrowth");
                    return;
                }
                var particleEmission = xanth.emission;
                particleEmission.rateOverTime = XANTH_EMISSION_RATE - (XANTH_EMISSION_RATE * (1 - tempStress));
            }
        }
    }

    public const float XANTH_EMISSION_RATE = 5f, XANTH_EVACUATION_RATE = 0.03f;
    private float xanthLerp;

    Material coralMat;
    Color coralColor = Color.black;
    ParticleSystem xanth;

    void Awake()
    {
        Branch = Resources.Load("Prefabs/Coral") as GameObject;
        coralMat = Material.Instantiate(Resources.Load("Materials/Coral") as Material);
        Branch.GetComponent<MeshRenderer>().material = coralMat;
        xanth = GetComponentInChildren<ParticleSystem>();
        RightAxis = true;
        coralSize = 0;
    }

	// Use this for initialization
	void Start () 
    {
        if (coralColor == Color.black)
            coralColor = Random.Range(0f, 1f) > 0.5f ? Color.yellow : Color.red;
        this.GetComponent<MeshRenderer>().material.color = coralColor;
        
        var particleMain = xanth.main;
        particleMain.startColor = coralColor;
        var particleEmission = xanth.emission;
        particleEmission.rateOverTime = 0;

        StartCoroutine("CoralGrowth");
	}
	
	// Update is called once per frame
	void Update () 
    {
        /*if (bleached)
        {
            if (Time.time - bleachTime > BLEACH_DEATH)
            {
                reef.RemoveCoral(this);
                GameObject.Destroy(this.gameObject);
            }
        }*/
	}

    public string Touch()
    {
        return "Coral";
    }

    IEnumerator CoralGrowth()
    {
        while (true)
        {
            yield return new WaitForSeconds(CORAL_GROWTH);
            if (coralSize >= maxGrowth)
            {
                StopCoroutine("CoralGrowth");
                yield break;
            }
            BranchCoral();
            CoralSize += growthRate;
        }
    }

    IEnumerator Evacuation()
    {
        xanthLerp = 0f;
        while (true)
        {
            yield return new WaitForSeconds(XANTH_EVACUATION_RATE);
            this.GetComponent<MeshRenderer>().material.color = Color.Lerp(coralColor, Color.white, xanthLerp);
            if (xanthLerp > 1f)
            {
                Destroy(xanth);
                StopCoroutine("Evacuation");
                yield break;
            }
            xanthLerp += Time.deltaTime;
        }
    }

    //Bleaching needs to be a discrete event with an animation provided by particle system
    //Sensitivity to bleaching determined by branching generation
    //1 - MAX_BRANCHES / Generation = TempSensitivity
    //
    public void StressCoral(float heatStress, Reef reeeeeeef)
    {
        reef = reeeeeeef;
        TemperatureStress = heatStress;
    }

    void BranchCoral()
    {
        if (Random.Range(0f, 1f) <= BRANCH_PROB && !branched)
        {
            branched = true;
            GameObject branch1 = GameObject.Instantiate(Branch);
            GameObject branch2 = GameObject.Instantiate(Branch);

            Vector3 swapAxis;
            if (RightAxis)
                swapAxis = this.transform.right;
            else
                swapAxis = this.transform.forward;

            branch1.transform.parent = this.transform.parent;
            branch2.transform.parent = this.transform.parent;

            branch1.transform.rotation = this.transform.rotation;
            branch2.transform.rotation = this.transform.rotation;
            branch1.transform.Rotate(swapAxis, -45f);
            branch2.transform.Rotate(swapAxis, 45f);
            branch1.transform.position = this.transform.position + (this.transform.up * this.transform.localScale.y);// +swapAxis * this.totalGrowth;
            branch2.transform.position = this.transform.position + (this.transform.up * this.transform.localScale.y);// -swapAxis * this.totalGrowth;
            branch1.transform.position += branch1.transform.up * branch1.transform.localScale.y/2f;
            branch2.transform.position += branch2.transform.up * branch2.transform.localScale.y/2f;
            branch1.transform.localScale = this.transform.localScale * CHILD_PROPORTION;
            branch2.transform.localScale = this.transform.localScale * CHILD_PROPORTION;

            ModifyBranch(branch1.GetComponent<Coral>());
            ModifyBranch(branch2.GetComponent<Coral>());
        }
    }

    private void ModifyBranch(Coral child)
    {
        child.Generation = generation + 1;
        child.RightAxis = !RightAxis;
        child.MaxGrowth = this.maxGrowth * CHILD_PROPORTION;
        child.GrowthRate = this.growthRate * CHILD_PROPORTION;
        child.coralColor = this.coralColor;

        child.StressCoral(this.tempStress, reef);
        reef.AddCoral(child);
    }

    void OnEnable()
    {
        StartCoroutine("CoralGrowth");
    }

    void OnDisable()
    {
        StopCoroutine("CoralGrowth");
    }
}
