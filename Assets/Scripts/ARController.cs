using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Method for AR scaling content adapted from Unity3d ARKit team 
//https://blogs.unity3d.com/2017/11/16/dealing-with-scale-in-ar/
//https://forum.unity.com/threads/scaled-content.500620/

public class ARController : MonoBehaviour
{
    public Camera ARCamera, ContentCamera;
    public GameObject InputGuide;

    private float contentScale;
    public const float MIN_SCALE = 0.01f, MAX_SCALE = 0.1f;
    public const float CONTENT_POSITION_SPEED = 1f;
    public const float CONTENT_ROTATE_THRESHOLD = 2.5f, CONTENT_ROTATE_SPEED = 0.1f;

    public string CurrentTrackedEcosystem = "None";
    private EcosystemController ecosystem;

    void Awake()
    {
        //ARCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
    }

    void Start()
    {
        //Set to render only background plane
        ARCamera.transform.GetChild(0).gameObject.layer = 8;
        CurrentTrackedEcosystem = "None";
        contentScale = 0.1f;
    }

    void Update()
    {
        if (ecosystem != null)
        {
            float invScale = 1.0f / contentScale;
            Vector3 anchorVec = ARCamera.transform.position - ecosystem.transform.position;
            
            float angleAdjust = Quaternion.Angle(ContentCamera.transform.rotation, ARCamera.transform.rotation);
            if (angleAdjust > CONTENT_ROTATE_THRESHOLD)
            {
                ContentCamera.transform.position = Vector3.Lerp(ContentCamera.transform.position, ecosystem.transform.position + (anchorVec * invScale), Time.deltaTime * CONTENT_POSITION_SPEED);
                ContentCamera.transform.rotation = Quaternion.Slerp(ContentCamera.transform.rotation, ARCamera.transform.rotation, Time.deltaTime * angleAdjust * CONTENT_ROTATE_SPEED);
            }
            //ContentCamera.transform.LookAt(ecosystem.transform.position);
        }
    }

    public GameObject LoadScene(GameObject target)
    {
        GameObject scene, canvas;
        if (CurrentTrackedEcosystem != "None")
        {
            InputGuide.transform.parent = null;
            GameObject lastEcosystem = GameObject.Find(CurrentTrackedEcosystem);
            scene = lastEcosystem.transform.GetChild(0).gameObject;
            canvas = lastEcosystem.transform.GetChild(1).gameObject;
            GameObject.Destroy(scene);
            GameObject.Destroy(canvas);
        }
        scene = GameObject.Instantiate(Resources.Load("Prefabs/Scenes/" + target.name) as GameObject);

        CurrentTrackedEcosystem = target.name;
        scene.transform.SetParent(target.transform, false);
        scene.transform.localPosition = Vector3.zero;
        
        ecosystem = scene.GetComponent<EcosystemController>();
        ecosystem.Guide = InputGuide;
        ecosystem.InputCamera = ContentCamera;
        canvas = ecosystem.EcosystemCanvas.gameObject;
        canvas.transform.SetParent(target.transform);

        return scene;
    }

    //Adds a value from input to the scene scale
    public void ScaleScene(float scale)
    {
        contentScale += scale;
        contentScale = Mathf.Clamp(contentScale, MIN_SCALE, MAX_SCALE);
    }

}
