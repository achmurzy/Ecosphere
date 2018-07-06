using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabitatController : MonoBehaviour, IPhotosensitive, ITouchable
{
    public SolController Sol;

    private float pressureLerp = 0, flashCounter = 0, flashBuffer = 2f, flashLerp = 0, deflationTimer = 0, nightPressure;
    public float HabitatPressure { get { return pressureLerp; } set { pressureLerp = value; heatEmitter.EmissionRate = 1 - pressureLerp; } }
    bool deflate = false, unflashed = true;

    public const float LIGHT_QUANTUM = 0.005f, FLASH = 0.001f;
    private const float SCALE_MIN = 1f, SCALE_MAX = 2f, SCALE_LERP_SPEED = 1f;
    private Vector3 baseScale;

    private Material luxMat, habMat;
    private Color luxYellow, luxOutline, habRed;

    Emitter heatEmitter;

    void Awake()
    {
        baseScale = this.transform.localScale;
        heatEmitter = GetComponent<Emitter>();

        Sol = FindObjectOfType<SolController>();
    }

	// Use this for initialization
	void Start () 
    {
        habMat = Material.Instantiate(Resources.Load("Materials/Biodome") as Material);
        luxMat = Material.Instantiate(Resources.Load("Materials/Lux") as Material);
        luxYellow = luxMat.color;
        luxOutline = luxMat.GetColor("_OutlineColor");
        habRed = habMat.color;
        GetComponent<MeshRenderer>().material = habMat;
        habMat.color = Color.white;

        Bounds emissionBounds = new Bounds();
        emissionBounds.min = -heatEmitter.gameObject.transform.localScale / 2;
        emissionBounds.max = heatEmitter.gameObject.transform.localScale / 2;
        heatEmitter.LowerEmissionTrajectory = new Vector3(0, 0, 0);
        heatEmitter.UpperEmissionTrajectory = new Vector3(0, 1, 0);
        heatEmitter.SpatialExtent = emissionBounds;

        heatEmitter.DestructionTrigger.size = new Vector3(1, 2, 1);
        heatEmitter.EmissionForce = Panel.HEAT_FLUX_FORCE;
        heatEmitter.EmissionRate = 1 - pressureLerp;
        heatEmitter.StartEmitter();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (deflate)
        {
            deflationTimer += Time.deltaTime;
            pressureLerp = nightPressure - ((deflationTimer / Sol.NightLength) * nightPressure);
            
            if (deflationTimer >= Sol.NightLength)
            {
                pressureLerp = 0;
                deflate = false;
            }
        }
        else
        {
            pressureLerp = Mathf.Clamp01(pressureLerp + (LIGHT_QUANTUM * Sol.SolarPeriod));
        }
        transform.localScale = Vector3.Lerp(baseScale * SCALE_MIN, baseScale * SCALE_MAX, pressureLerp);
        if(unflashed)
            habMat.color = Color.Lerp(Color.white, habRed, pressureLerp);
	}

    public void Deflate()
    {
        TerminateFlash();

        deflate = true;
        deflationTimer = 0f;
        nightPressure = pressureLerp;
    }

    public string Touch()
    {
        return "Habitat zone";
    }

    public bool LightEnter(SolarRay ray)
    {
        if (Time.time - flashCounter > flashBuffer)
        {
            GetComponent<MeshRenderer>().material = luxMat;
            flashLerp = 0f;
            StartCoroutine("Flash");
            unflashed = false;
            return true;
        }
        return false;
    }

    IEnumerator Flash()
    {
        while (true)
        {
            yield return new WaitForSeconds(FLASH);
            luxMat.color = Color.Lerp(luxYellow, habMat.color, flashLerp);
            luxMat.SetColor("_OutlineColor", Color.Lerp(Color.white, luxOutline, flashLerp));

            if (flashLerp >= 1f)
            {
                TerminateFlash();
                yield break;
            }
            flashLerp += Time.deltaTime;
        }
    }

    private void TerminateFlash()
    {
        unflashed = true;
        luxMat.color = Color.yellow;
        GetComponent<MeshRenderer>().material = habMat;
        flashCounter = Time.time;
        flashLerp = 0f;
        StopCoroutine("Flash");
    }
}
