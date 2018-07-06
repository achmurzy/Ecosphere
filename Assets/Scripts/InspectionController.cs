using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectionController : MonoBehaviour {

    private GameObject anchorObject;
    private Camera renderCamera;
    private Vector3 anchorPosition;

    private Text inspectionText;
    public const float LERP_DISTANCE = 0.1f;

    void Awake()
    {
        inspectionText = GetComponentInChildren<Text>();
    }

	// Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.localPosition = Vector3.Lerp(anchorPosition + (renderCamera.transform.up.normalized*LERP_DISTANCE), anchorPosition - (renderCamera.transform.up.normalized*LERP_DISTANCE), (Mathf.Sin(Time.time) + 1)/2);
        transform.LookAt(renderCamera.transform, renderCamera.transform.up);
        transform.Rotate(transform.up, 180f, Space.World);
	}

    //Fix the canvas to a world space object in view of a camera
    public void FixCanvas(GameObject anchor, Camera renderer)
    {
        anchorObject = anchor;
        transform.parent = anchor.transform;
        transform.localPosition = anchor.transform.up.normalized;// *anchor.transform.localScale.y;
        anchorPosition = transform.localPosition;

        renderCamera = renderer;
        //transform.LookAt(renderer.transform, renderCamera.transform.up);
        //transform.Rotate(transform.up, 180f);

        //Panel generally scaled down to scene level with this coefficient
        GameObject inspectionPanel = transform.GetChild(0).gameObject;
        inspectionPanel.GetComponent<Image>().enabled = true;
        inspectionPanel.transform.localScale = Vector3.one * (0.01f);
        
        inspectionText.text = anchor.tag;
        inspectionText.enabled = true;
    }
}
