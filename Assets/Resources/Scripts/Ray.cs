using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour {

    float rayLerp = 0;
    float lerpSpeed=2f;
    float rayWidth = 0.1f;
    float spatialExtent = 1f;

    GameObject Sol, Sunflower;
    Vector3 origPos, origScale;
    float solDist;

	// Use this for initialization
	void Start () 
    {
        Sunflower = GameObject.Find("Sunflower");
        this.transform.LookAt(Sunflower.transform.position);
        this.transform.Rotate(90, 0, 0);
        Vector2 offset = Random.insideUnitCircle*spatialExtent;
        this.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y + offset.y, this.transform.position.z);
        this.transform.localScale = new Vector3(rayWidth, 1, rayWidth);
        origScale = this.transform.localScale;

        
        origPos = this.transform.position;
        solDist = Vector3.Distance(Sunflower.transform.position, this.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        rayLerp += Time.deltaTime * lerpSpeed;
        if (rayLerp >= 1f)
            GameObject.Destroy(this.gameObject);
        this.transform.position = Vector3.Lerp(origPos, origPos + (this.transform.up * solDist * 0.5f), rayLerp);
        this.transform.localScale = Vector3.Lerp(origScale, new Vector3(0, solDist * 0.25f, 0), rayLerp);
	}
}
