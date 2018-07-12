using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Consider implementing quantum yield to model crown transmissivity, and connect to ray params
public class SolarRay : MonoBehaviour {

    float rayLerp = 0;
    public const float LIGHT_SPEED = 1f;
    public const float RAY_WIDTH = 0.5f, RAY_LENGTH = 0.76f;

    public SolController Sol;
    public Vector3 origPos, goalPos, origScale, goalScale;
    float solDist;

    bool transmit = true;

	// Use this for initialization
	void Start () 
    {   
	}
	
	// Update is called once per frame
	void Update () {
        rayLerp += Time.deltaTime * LIGHT_SPEED;
        if (rayLerp >= 1f)
            GameObject.Destroy(this.gameObject);

        if (!transmit)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, rayLerp);
        }
        else
        {
            this.transform.localPosition = Vector3.Lerp(origPos, goalPos, rayLerp);
            this.transform.localScale = Vector3.Lerp(origScale, goalScale, rayLerp);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        IPhotosensitive photo = other.GetComponent<IPhotosensitive>();
        if(photo != null && transmit)
        {
            if(photo.LightEnter(this))
                transmit = false;
        }
    }
}
