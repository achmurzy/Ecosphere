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

    private Vector3 savePos;
    private Quaternion saveRot;
    private float contentScale, saveScale;
    public const float MIN_SCALE = 0.01f, MAX_SCALE = 0.1f;
    public const float CONTENT_POSITION_SPEED = 2.5f, CONTENT_INSPECT_DISTANCE = 5f;
    public const float CONTENT_ROTATE_THRESHOLD = 2.5f, CONTENT_ROTATE_SPEED = 0.1f;

    public EcosystemEventHandler Trackable;
    private EcosystemController ecosystem;

    void Awake()
    {
        //ARCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
    }

    void Start()
    {
        //Set to render only background plane
        ARCamera.transform.GetChild(0).gameObject.layer = 8;
        Trackable = null;
        contentScale = 0.01f;
        saveScale = contentScale;

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Update()
    {
        if (ecosystem != null)
        {
            if (ecosystem.Inspecting)
            {}
            else if (Trackable.State == Vuforia.TrackableBehaviour.Status.NO_POSE)
            {
                //Return content camera to the screen center using the camera feed background plane
                float invScale = 1.0f / contentScale;
                Transform backgroundPlane = ARCamera.transform.GetChild(0);
                Vector3 anchorVec = -(backgroundPlane.position - ecosystem.transform.position).normalized;
                ContentCamera.transform.position = Vector3.Lerp(ContentCamera.transform.position, ecosystem.transform.position + (anchorVec * invScale), Time.deltaTime * CONTENT_POSITION_SPEED);
                ContentCamera.transform.LookAt(backgroundPlane);
                //ContentCamera.transform.rotation = Quaternion.Slerp(ContentCamera.transform.rotation, Quaternion.identity, Time.deltaTime * CONTENT_ROTATE_SPEED);
                /*float angleAdjust = Quaternion.Angle(ContentCamera.transform.rotation, ARCamera.transform.rotation);
                if (angleAdjust > CONTENT_ROTATE_THRESHOLD)
                {
                    ContentCamera.transform.rotation = Quaternion.Slerp(ContentCamera.transform.rotation, ARCamera.transform.rotation, Time.deltaTime * angleAdjust * CONTENT_ROTATE_SPEED);
                }*/
            }
            else
            {
                float invScale = 1.0f / contentScale;
                Vector3 anchorVec = (ARCamera.transform.position - ecosystem.transform.position).normalized;
                /*Debug.Log(invScale);
                Debug.Log(ContentCamera.transform.position);
                Debug.Log(anchorVec);
                Debug.Log(ecosystem.transform.position);*/
                Vector3 inf = Vector3.Lerp(ContentCamera.transform.position, ecosystem.transform.position + (anchorVec * invScale), Time.deltaTime * CONTENT_POSITION_SPEED);

                ContentCamera.transform.position = inf;

                float angleAdjust = Quaternion.Angle(ContentCamera.transform.rotation, ARCamera.transform.rotation);
                if (angleAdjust > CONTENT_ROTATE_THRESHOLD)
                {
                    ContentCamera.transform.rotation = Quaternion.Slerp(ContentCamera.transform.rotation, ARCamera.transform.rotation, Time.deltaTime * angleAdjust * CONTENT_ROTATE_SPEED);
                }

            }
            ScaleScene(Input.mouseScrollDelta.y * MIN_SCALE);
        }
    }

    public GameObject LoadScene(EcosystemEventHandler target)
    {
        GameObject scene, canvas;
        if (Trackable != null)
        {
            if(ecosystem != null)
            {   /*ecosystem.StopInspecting();*/ }
            else
            {   Debug.Log("Ecosystem not found with trackable"); }
            InputGuide.transform.parent = null;
            scene = Trackable.transform.GetChild(0).gameObject;
            canvas = Trackable.transform.GetChild(1).gameObject;
            GameObject.Destroy(scene);
            GameObject.Destroy(canvas);
        }
        scene = GameObject.Instantiate(Resources.Load("Prefabs/Scenes/" + target.name) as GameObject);

        Trackable = target;
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

    public void InspectScene(GameObject obj)
    {
        saveRot = ContentCamera.transform.rotation;
        savePos = ContentCamera.transform.position;
        saveScale = contentScale;

        //Position camera based on object - should be function of object scale ideally - scale factor based on magnitude of scale in direction of camera
        float scaleFactor = Vector3.Dot(obj.transform.localScale, (obj.transform.position - ContentCamera.transform.position).normalized);
        ContentCamera.transform.position = obj.transform.position - (ContentCamera.transform.forward.normalized * (CONTENT_INSPECT_DISTANCE + obj.transform.localScale.magnitude));

        //Save inspection object for rotation, scaling functionality?
    }

    public void StopInspecting()
    {
        contentScale = saveScale;
        ContentCamera.transform.position = savePos;
        ContentCamera.transform.rotation = saveRot;
    }

}
