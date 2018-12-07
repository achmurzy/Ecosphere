using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolController : MonoBehaviour, ITouchable
{
    private GameObject Pivot, Light;
    GameObject Beam;
    public float RadiationIntensity, SolarPeriod = 0f;
    
    public float SolDistance = 50, MinimumAltitude = 20;
    public Vector2 SolarExtent;

    //Rate of day reset
    public float NightLength = 2f;
    public float RotationDelta { get { return (180 + (2 * MinimumAltitude))/NightLength; } }
    private float nightLerp, pivotTilt;
    private Quaternion sunriseRotation, sunsetRotation, horizon;

    public const float EXTENT_SCALING = 1f;
    private bool rotate = false, crepuscular = false;

    public string Touch() { return "Sol"; }

    void Awake()
    {
        Pivot = transform.parent.gameObject;
        pivotTilt = Pivot.transform.rotation.eulerAngles.y;
        horizon = Pivot.transform.rotation;
        Light = transform.GetChild(0).gameObject;
        transform.localPosition = Pivot.transform.up * SolDistance;
        Beam = Resources.Load("Prefabs/Ray") as GameObject;
        GetComponentInChildren<Light>().transform.rotation = Quaternion.identity; 
    }
    
    void Start () 
    {
        //SolarExtent = new Vector2(this.transform.localScale.x, this.transform.localScale.z);
        Sunrise();
	}

	// Update is called once per frame
	void Update () 
    {
        if (rotate)
        {
            if (crepuscular)
            {
                Pivot.transform.Rotate(Pivot.transform.forward.normalized * RotationDelta * Time.deltaTime, Space.World);
                nightLerp += Time.deltaTime;
                if (nightLerp >= NightLength)
                {
                    crepuscular = false;
                    nightLerp = 0;
                    Light.GetComponent<Light>().enabled = true;
                    Sunrise();
                }
            }
            else
            {
                float altitude = Vector3.Angle(Pivot.transform.up, Quaternion.AngleAxis(pivotTilt, Pivot.transform.parent.up) * Pivot.transform.parent.right);
                Pivot.transform.Rotate(Pivot.transform.forward * SolarPeriod * 0.5f, Space.World);
                if (altitude > 180f - MinimumAltitude)
                {
                    Light.GetComponent<Light>().enabled = false;
                    crepuscular = true;
                    sunriseRotation = Quaternion.FromToRotation(Pivot.transform.up, -Pivot.transform.up);
                    sunsetRotation = Pivot.transform.rotation;
                    Sunset();
                    SendMessageUpwards("Night", SendMessageOptions.DontRequireReceiver);
                }
            }         
        }

        transform.LookAt(Vector3.zero);
	}

    void AddRay()
    {
        GameObject newRay = GameObject.Instantiate(Beam) as GameObject;
        SolarRay rray = newRay.GetComponent<SolarRay>();
        rray.Sol = this;

        newRay.transform.parent = this.Pivot.transform.parent;
        newRay.transform.forward = this.transform.forward;
        newRay.transform.Rotate(90, 0, 0);
        
        Vector2 offset = Random.insideUnitCircle;
        offset = new Vector2(offset.x * SolarExtent.x, offset.y * SolarExtent.y);
        newRay.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y, this.transform.position.z + offset.y);

        newRay.transform.localScale = new Vector3(SolarRay.RAY_WIDTH, SolarRay.RAY_LENGTH, SolarRay.RAY_WIDTH) * RadiationIntensity;

        rray.origScale = newRay.transform.localScale;
        rray.origPos = newRay.transform.localPosition;
        rray.goalPos = new Vector3(offset.x, 0, offset.y);// rray.origPos + (Vector3.up * SolDistance);
        rray.goalScale = new Vector3(0, SolDistance * RadiationIntensity * SolarRay.RAY_LENGTH, 0);
    }

    IEnumerator Shine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 - RadiationIntensity);
            AddRay();
        }
    }

    public void Diurnal()
    {
        rotate = true;
    }

    public void Sunrise()
    {
        StartCoroutine("Shine");
    }
    public void Sunset()
    {
        StopCoroutine("Shine");
    }
    
    public void AttenuateLight(float val)
    {
        RadiationIntensity = val;
    }

    public void ChangePhotoperiod(float val)
    {
        SolarPeriod = val;
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
