using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sky : MonoBehaviour 
{
    GameObject cloud;
    public const int CLOUD_COUNT = 4;
    private List<Cloud> firmament;
    public Cloud[] Firmament { get { return firmament.ToArray(); } }
    private Stand stand;

    SolController sol;
    public Slider WaterSlider;
    public const float PRECIP_RATE_MIN = 2f, PRECIP_RATE_MAX = 0.1f;


    void Awake()
    {
        cloud = Resources.Load("Prefabs/Cloud") as GameObject;
        firmament = new List<Cloud>();
        stand = FindObjectOfType<Stand>();
        sol = FindObjectOfType<SolController>();
    }
    
    // Use this for initialization
	void Start () 
    {
        	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Monsoon()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (firmament.Count < CLOUD_COUNT)
            {
                GameObject nimbus = GameObject.Instantiate(cloud);
                nimbus.transform.SetParent(this.gameObject.transform, false);
                nimbus.transform.localPosition = new Vector3(Random.Range(-ForestController.FOREST_X, ForestController.FOREST_X), 
                        Random.Range(sol.SolDistance / 2, 5*sol.SolDistance / 8), Random.Range(-ForestController.FOREST_Y, ForestController.FOREST_Y));
                Cloud cc = nimbus.GetComponent<Cloud>();
                cc.CloudSky = this;
                firmament.Add(cc);
            }
        }
    }

    public void SetPrecipitation()
    {
        float precip = Mathf.Lerp(PRECIP_RATE_MIN, PRECIP_RATE_MAX, WaterSlider.value);
        foreach (Cloud cc in firmament)
        {
            cc.PrecipitationRate = precip;
        }
    }

    void OnEnable()
    {
        StartCoroutine("Monsoon");
        //What happens if atmo is enabled while vegetation is disabled?
        /*foreach (Emitter em in stand.GetComponentsInChildren<Emitter>())
        {
            em.StartExchanger();
        }*/
    }

    void OnDisable()
    {
        StopCoroutine("Monsoon");
        //Disable all vegetation emitters, which create atmospheric components
        foreach (Emitter em in stand.GetComponentsInChildren<Emitter>())
        {
            em.StopExchanger();
        }
    }

    public void RemoveCloud(Cloud cc)
    {
        firmament.Remove(cc);
    }
}
