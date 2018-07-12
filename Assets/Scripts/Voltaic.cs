using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voltaic : MonoBehaviour, IPhotosensitive 
{
    private const float VOLT_FLASH = 0.01f, VOLT_WIDTH = 0.03f, VOLT_SCALE = 2f;
    Material luxMat, voltMat;
    Color luxYellow, luxOutline, voltBlack;

    private float voltFlashLerp, voltFlashCounter, voltFlashBuffer = 1f;
    private float startSize;
    private Panel panel;

    void Awake()
    {
        voltMat = GetComponent<MeshRenderer>().material;
        luxMat = Material.Instantiate(Resources.Load("Materials/Lux") as Material);
        luxYellow = luxMat.color;
        luxOutline = luxMat.GetColor("_OutlineColor");
        voltBlack = voltMat.color;
    }

	// Use this for initialization
	void Start () 
    {
        panel = GetComponentInParent<Panel>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool LightEnter(SolarRay ray)
    {
        if (Time.time - voltFlashCounter > voltFlashBuffer && voltFlashLerp == 0f)
        {
            GetComponent<MeshRenderer>().material = luxMat;
            startSize = VOLT_WIDTH*2;
            StartCoroutine("VoltFlash");
            panel.Volt();
            return true;
        }
        return false;
    }

    IEnumerator VoltFlash()
    {
        while (true)
        {
            yield return new WaitForSeconds(VOLT_FLASH);
            luxMat.color = Color.Lerp(luxYellow, voltBlack, voltFlashLerp);
            luxMat.SetColor("_OutlineColor", Color.Lerp(Color.white, luxOutline, voltFlashLerp));

            this.transform.localScale = Vector3.Lerp(Vector3.one * VOLT_WIDTH * VOLT_SCALE, Vector3.one * VOLT_WIDTH, voltFlashLerp);

            if (voltFlashLerp >= 1f)
            {
                luxMat.color = Color.yellow;
                GetComponent<MeshRenderer>().material = voltMat;
                voltFlashCounter = Time.time;
                voltFlashLerp = 0f;
                StopCoroutine("VoltFlash");
                yield break;
            }
            voltFlashLerp += Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
}
