using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour 
{
    public GameObject Panel0, Panel1, Panel2, Panel3;
    private List<GameObject> rotationPanels;
    public const float PANEL_ROTATION_LIMIT = 45.0f;
    public Slider RotationSlider;

    void Awake()
    {
        rotationPanels = new List<GameObject>();
    }

	// Use this for initialization
	void Start () 
    {
        Panel0.transform.localPosition = new Vector3(AgrivoltaicController.AGRIVOLT_X, 0, AgrivoltaicController.AGRIVOLT_Y);
        Panel1.transform.localPosition = new Vector3(AgrivoltaicController.AGRIVOLT_X, 0, -AgrivoltaicController.AGRIVOLT_Y);
        Panel2.transform.localPosition = new Vector3(-AgrivoltaicController.AGRIVOLT_X, 0, -AgrivoltaicController.AGRIVOLT_Y);
        Panel3.transform.localPosition = new Vector3(-AgrivoltaicController.AGRIVOLT_X, 0, AgrivoltaicController.AGRIVOLT_Y);

        foreach (Emitter em in GetComponentsInChildren<Emitter>())
        {
            rotationPanels.Add(em.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public void RotatePanels()
    {
        foreach (GameObject go in rotationPanels)
        {
            //go.gameObject.transform.rotation = Quaternion.AngleAxis(RotationSlider.value, go.gameObject.transform.forward);
            //go.gameObject.transform.Rotate(0, 0, RotationSlider.value);
            //go.gameObject.transform.localRotation = Quaternion.AngleAxis(RotationSlider.value, go.gameObject.transform.forward);
            //go.transform.eulerAngles.z = RotationSlider.value;

            go.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, RotationSlider.value));
        }
    }
}
