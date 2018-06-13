using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

//The Ocean needs to be an Aqua cube. Make it
public class OceanController : MonoBehaviour {

    Vector3 oceanExtent = new Vector3(10, 10, 10);
    List<Coral> reef;
    public const int REEF_CAPACITY = 25, SUCCESSION_RATE = 3;
    public GameObject OceanSurface, Seawater, Sol;

    public Slider HeatSlider, AcidSlider;   //CO2 is really driving these. Can't we just use that?
    public float OceanSurfaceTemperature { get; set; }
    public float OceanAcidity { get; set; }
    public const float PARTICLE_ACIDIDTY = 0.01f;

    void Awake()
    {
        reef = new List<Coral>();
        Sol = GameObject.Find("Sol");
        OceanSurface = GameObject.Find("Water");
        Seawater = GameObject.Find("Seawater");
    }

    // Use this for initialization
    void Start()
    {
        Seawater.transform.localScale = oceanExtent*2;
        Seawater.transform.localPosition = new Vector3(0, oceanExtent.y, 0);

        OceanSurface.transform.localScale = oceanExtent * 2/10;
        OceanSurface.transform.localPosition = new Vector3(0, (oceanExtent.y * 2) + 0.01f, 0);
        OceanSurface.GetComponent<Emitter>().SpatialCenter = new Vector3(0, -5, 0);
        OceanSurface.GetComponent<Emitter>().SpatialExtent = new Vector3(oceanExtent.x, 0, oceanExtent.y);
        OceanSurface.GetComponent<Emitter>().DestructionTrigger.size = new Vector3(oceanExtent.x, 5, oceanExtent.y);
        OceanSurface.GetComponent<Emitter>().EmissionForce = 0;
        OceanSurface.GetComponent<Emitter>().EmissionTrajectory = new Vector3();
        OceanSurface.GetComponent<Emitter>().StartExchanger();

        StartCoroutine("Succession");
    }

    // Update is called once per frame
    void Update()
    { }

    IEnumerator Succession()
    {
        while (true)
        {
            yield return new WaitForSeconds(SUCCESSION_RATE);
            if (reef.Count < REEF_CAPACITY)
            {
                GameObject coral = GameObject.Instantiate(Resources.Load("Prefabs/Coral")) as GameObject;
                coral.transform.parent = this.gameObject.transform;
                coral.transform.localPosition = new Vector3(Random.RandomRange(-oceanExtent.x, oceanExtent.x), 0, Random.RandomRange(-oceanExtent.y, oceanExtent.y));
                Coral newCoral = coral.GetComponent<Coral>();
                newCoral.StressCoral(HeatSlider.value);
                reef.Add(newCoral);
            }
        }
    }

    public void HeatOcean()
    {
        OceanSurfaceTemperature = HeatSlider.value;
        OceanSurface.GetComponent<Ocean>().LerpOceanColor(OceanSurfaceTemperature);
        BroadcastMessage("StressCoral", HeatSlider.value);
    }

    public void AcidifyOcean()
    {
        OceanAcidity += PARTICLE_ACIDIDTY;
        OceanSurface.GetComponent<Ocean>().LerpSeawaterColor(OceanAcidity);
    }

    public void RemoveCoral(Coral coral)
    {
        reef.Remove(coral);
    }
}
