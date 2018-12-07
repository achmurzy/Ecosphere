using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterSurface : MonoBehaviour 
{
    private float width, height, border;
    public float MinX, MaxX, MinY, MaxY, UIscaleFactor;
 
    public Image Surface;
    public Image Selector;

    public Color Dim1Color; //X
    public Color Dim2Color; //Y

    void Awake()
    {
        float match = GetComponentInParent<CanvasScaler>().matchWidthOrHeight;
        Vector2 refRes = GetComponentInParent<CanvasScaler>().referenceResolution;
        UIscaleFactor = ((Screen.width / refRes.x) * (1-match)) + ((Screen.height / refRes.y) * match);

        RectTransform rect = Surface.GetComponent<RectTransform>();
        width = rect.rect.width * UIscaleFactor;
        height = rect.rect.height * UIscaleFactor;
        border = (Selector.GetComponent<RectTransform>().sizeDelta[0] * UIscaleFactor) / 2f;
    }

   	// Use this for initialization
	void Start () 
    {    
        MinX = Screen.width - (width - border);
        MaxX = Screen.width - border;

        MinY = Screen.height - (height - border);
        MaxY = Screen.height - border;
    }
	
	// Update is called once per frame
	void Update () 
    {

	}

    public void UpdateButton()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 updateVec = Selector.transform.position;

        if (mouse.x < MaxX && mouse.x > MinX) 
            updateVec.x = Input.mousePosition.x;
        if (mouse.y < MaxY && mouse.y > MinY)
            updateVec.y = Input.mousePosition.y;

        Selector.transform.position = updateVec;
        UpdateParams();
    }

    public void ExitButton()
    {
        Selector.transform.position = Vector3.zero;
    }

    public void UpdateParams()
    {
        float dx = ((Selector.GetComponent<RectTransform>().anchoredPosition.x*UIscaleFactor) - border) / (width - (2 * border));
        float dy = ((Selector.GetComponent<RectTransform>().anchoredPosition.y*UIscaleFactor) - border) / (height- (2 * border));

        Color panelColor = ((Dim1Color * dx) + (Dim2Color * dy));
        panelColor.a = 1;
        Surface.color = panelColor;
        Vector2 res = new Vector2(dx, dy);

        SendMessageUpwards("ParameterShift", res);
    }
}
