using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunflowerController : MonoBehaviour {

    GameObject Head, Rosette, Sol, tText;
    float headSmooth, headSmoothVel, headSmoothSpeed = 0.5f;
    float rosSmooth, rosSmoothVel, rosSmoothSpeed = 0.75f;
    bool up1 = true;
    bool up2 = true;

    float scaleRange = 0.25f;
    float rotRange = 25f;

    GasExchanger Transpirator;

	// Use this for initialization
	void Start () 
    {
        Head = transform.GetChild(0).gameObject;
        Rosette = transform.GetChild(1).gameObject;
        Sol = GameObject.Find("Sol");
        tText = GameObject.Find("ToggleText");

        Transpirator = GetComponent<GasExchanger>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (up1)
            headSmooth = Mathf.SmoothDamp(headSmooth, scaleRange, ref headSmoothVel, headSmoothSpeed);
        else
            headSmooth = Mathf.SmoothDamp(headSmooth, -scaleRange, ref headSmoothVel, headSmoothSpeed);

        if (up2)
            rosSmooth = Mathf.SmoothDamp(rosSmooth, rotRange, ref rosSmoothVel, rosSmoothSpeed);
        else
            rosSmooth = Mathf.SmoothDamp(rosSmooth, -rotRange, ref rosSmoothVel, rosSmoothSpeed);

        headSmooth = Mathf.Clamp(headSmooth, -scaleRange, scaleRange);
        var val = scaleRange - Mathf.Abs(headSmooth);
        if (val < 0.005)
            up1 = !up1;

        rosSmooth = Mathf.Clamp(rosSmooth, -rotRange, rotRange);
        val = rotRange - Mathf.Abs(rosSmooth);
        if (val < 0.5)
            up2 = !up2;
        Head.transform.localScale = new Vector3(1f + (headSmooth), (1 + headSmooth), 1f + (headSmooth));
        Rosette.transform.localRotation = Quaternion.identity;
        Rosette.transform.Rotate(0, rosSmooth, 0);
	}

    public void ToggleText()
    {
        tText.GetComponent<Image>().enabled = !tText.GetComponent<Image>().enabled;
    }
}
