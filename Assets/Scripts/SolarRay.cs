using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Consider implementing quantum yield to model crown transmissivity, and connect to ray params
public class SolarRay : MonoBehaviour {

    float rayLerp = 0;
    public const float LIGHT_SPEED = 1f;
    public const float RAY_WIDTH = 0.05f;

    public SolController Sol;
    Vector3 origPos, origScale;
    float solDist;

    bool transmit = true;

	// Use this for initialization
	void Start () 
    {   
        origScale = this.transform.localScale;
        origPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        rayLerp += Time.deltaTime * LIGHT_SPEED;
        if (rayLerp >= 1f)
            GameObject.Destroy(this.gameObject);
        this.transform.position = Vector3.Lerp(origPos, origPos + (this.transform.up * Sol.SolDistance), rayLerp);
        this.transform.localScale = Vector3.Lerp(origScale, new Vector3(0, Sol.SolDistance * Sol.RadiationIntensity * 0.1f, 0), rayLerp);
	}

    void OnTriggerEnter(Collider other)
    {
        IPhotosensitive photo = other.GetComponent<IPhotosensitive>();
        if(photo != null && transmit)
        {
            if(photo.LightEnter())
                transmit = false;
        }
    }
}
