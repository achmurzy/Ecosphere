using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LungController : MonoBehaviour, ITouchable 
{
    private SolController Sol;
    public HabitatController Habitat;
    public GameObject Tunnel;
    bool deflate = false;

    private float pressureLerp = 0, deflationTimer = 0, nightPressure, nightPump;
    public float PumpRate, pumpLerp;
    private bool pump = false, pull = false;

    private const float PUMP_SCALE = 0.05f, PUMP_RATE = 0.05f;
    private const float SCALE_MIN = 1f, SCALE_MAX = 2f, TUNNEL_SCALE = 2f;
    private const float PULL_RATE = 2f, FILL_RATE = 0.005f;
    private Vector3 baseScale, tunnelScale;
    private Material habMat, tunnelMat;
    private Color habRed;

    void Awake()
    {
        habMat = Material.Instantiate(Resources.Load("Materials/Biodome") as Material);
        tunnelMat = Material.Instantiate(Resources.Load("Materials/Biodome") as Material);

        habRed = habMat.color;
        GetComponent<MeshRenderer>().material = habMat;
        Tunnel.GetComponent<MeshRenderer>().material = tunnelMat;

        baseScale = this.transform.localScale;
        tunnelScale = Tunnel.transform.localScale;
        Sol = FindObjectOfType<SolController>();
        Habitat = FindObjectOfType<HabitatController>();
    }

	// Use this for initialization
	void Start () 
    {
        StartCoroutine("Pump");
	}

    // Update is called once per frame
    void Update()
    {
        if (deflate)
        {
            deflationTimer += Time.deltaTime;
            pressureLerp = nightPressure - ((deflationTimer / Sol.NightLength) * nightPressure);
            pumpLerp = nightPump - ((deflationTimer / Sol.NightLength) * nightPump);
            if (deflationTimer >= Sol.NightLength)
            {
                pumpLerp = 0;
                pressureLerp = 0;
                deflate = false;
                StartCoroutine("Pump");
            }
        }
        else if (pull)
        {
            pumpLerp += Time.deltaTime * PULL_RATE * Sol.SolarPeriod;
            if (pumpLerp >= 1)
            {
                pump = true;
                pull = false;
            }
        }
        else if (pump)
        {
            pumpLerp -= Time.deltaTime * Sol.SolarPeriod;
            pressureLerp += (PumpRate * FILL_RATE);
            if (pumpLerp <= 0)
            {
                pump = false;
            }
        }

        transform.localScale = Vector3.Lerp(baseScale * SCALE_MIN, baseScale * SCALE_MAX, pressureLerp);
        habMat.color = Color.Lerp(Color.white, habRed, pressureLerp);

        Vector3 newScale = Vector3.Lerp(tunnelScale, tunnelScale * TUNNEL_SCALE, pumpLerp);
        Tunnel.transform.localScale = new Vector3(newScale.x, tunnelScale.y, newScale.z);
        tunnelMat.color = Color.Lerp(Color.white, habRed, pumpLerp);
    }

    IEnumerator Pump()
    {
        while (true)
        {
            yield return new WaitForSeconds(1/Sol.SolarPeriod * PUMP_RATE);
            if (Habitat.HabitatPressure > 0 && !pump && !pull)
            {
                pumpLerp = 0; 
                pull = true;
                Habitat.HabitatPressure = Mathf.Clamp01(Habitat.HabitatPressure - (PumpRate * PUMP_SCALE));
            }
            
        }
    }

    public void Deflate()
    {
        StopCoroutine("Pump");
        pull = false;
        pump = false;
        deflate = true;
        nightPressure = pressureLerp;
        nightPump = pumpLerp;
    }

    public void SlidePumpRate(float val)
    {
        PumpRate = val;
    }

    public string Touch()
    {
        return name;
    }
}
