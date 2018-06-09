using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : MonoBehaviour, IPhotosensitive 
{
    Tree wholePlant; 

    Material luxMat, crownMat;
    private const float CROWN_FLASH = 0.01f, H2O_SCALE = 0.1f, EMISSION_SCALING = 100f;
    private const float GROWTH_ITER = 0.1f;
    private float startWidth;
    private float crownFlashLerp = 0f, crownFlashBuffer = 1f, crownFlashCounter = 0f;
    Color crownGreen, luxYellow, luxOutline;

    Emitter transpirator;
    private float crownRadius;
    public float CrownRadius
    {
        get { return crownRadius; }
        set
        {
            crownRadius = value;
            transform.localScale = Vector3.one * crownRadius;
            transform.localPosition = new Vector3(0, 2 * GetComponentInParent<Tree>().StemHeight, 0);
            transpirator.SpatialExtent = Vector3.one * crownRadius;
            transpirator.DestructionTrigger.size = Vector3.one * crownRadius * 2f;
            transpirator.EmissionRate = 1 / crownRadius * EMISSION_SCALING;
        }
    }

    void Awake()
    {
        transpirator = GetComponentInChildren<Emitter>();
        transpirator.Molecule.GetComponent<Molecule>().MolecularScale = H2O_SCALE;

        crownMat = GetComponent<MeshRenderer>().material;
        luxMat = Material.Instantiate(Resources.Load("Materials/Lux") as Material);
        luxYellow = luxMat.color;
        luxOutline = luxMat.GetColor("_OutlineColor");
        crownGreen = crownMat.color;

        wholePlant = GetComponentInParent<Tree>();
    }

	// Use this for initialization
	void Start () {
        transpirator.StartExchanger();
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public bool LightEnter()
    {
        if (Time.time - crownFlashCounter > crownFlashBuffer && crownFlashLerp == 0f)
        {
            GetComponent<MeshRenderer>().material = luxMat;
            startWidth = wholePlant.StemWidth;
            StartCoroutine("CrownFlash");
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
            
            if (crownFlashLerp >= 1f)
            {
                luxMat.color = Color.yellow;
                GetComponent<MeshRenderer>().material = crownMat;
                crownFlashCounter = Time.time;
                crownFlashLerp = 0f;
                StopCoroutine("CrownFlash");
                yield break;
            }
            crownFlashLerp += Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
