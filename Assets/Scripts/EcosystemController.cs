using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EcosystemController : MonoBehaviour 
{
    private GameObject inputGuide;
    public GameObject Guide { get { return inputGuide; } set { inputGuide = value; inputGuide.transform.parent = this.transform.parent; inputGuide.transform.localPosition = Vector3.zero; } }
    public Camera InputCamera { get; set; }

    public Canvas EcosystemCanvas;
    public Image ControlImage;
    protected Sprite pause, play;
    private bool paused;

    private bool dragged = false;
    private Vector2 lastMouse;

    public const float MAX_ROT_SPEED = 2.5f, MAX_SCALE_SPEED = 0.01f;
    public float repositionTime = 0f, lastDist;
    public const float REPOSITION_LIMIT = 5.0f;
    public const float REPOSITION_SPEED = 5f;

    protected void Awake()
    {
        EcosystemCanvas = GetComponentInChildren<Canvas>();
        pause = Resources.Load("Images/pause", typeof(Sprite)) as Sprite;
        play = Resources.Load("Images/play", typeof(Sprite)) as Sprite;
    }

	// Use this for initialization
	protected void Start () 
    {
        InputCamera = Camera.main;
        Guide = GameObject.Find("InputGuide");
	}
	
	// Update is called once per frame
	protected void Update () 
    {
        if (Guide != null)
            Guide.transform.LookAt(InputCamera.transform.position, InputCamera.transform.up);
        
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            bool interactingUI = false;
            if (Input.touchCount == 0)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    interactingUI = true;
            }
            else
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (EventSystem.current.IsPointerOverGameObject(i))
                    {
                        interactingUI = true;
                        break;
                    }
                }
            }

            if (!interactingUI)
            {
                if (Input.touchCount > 1) //Scaling
                {
                    Touch tt1 = Input.GetTouch(0);
                    Touch tt2 = Input.GetTouch(1);
                    float dist = Vector2.Distance(tt1.position, tt2.position);

                    if (tt1.phase == TouchPhase.Began || tt2.phase == TouchPhase.Began)
                    { }
                    else if (tt1.phase == TouchPhase.Moved || tt2.phase == TouchPhase.Moved)
                    {
                        float sign = Mathf.Sign(dist - lastDist);
                        FindObjectOfType<ARController>().ScaleScene(MAX_SCALE_SPEED * sign);
                    }
                    lastDist = dist;
                }
                else
                {
                    bool raycastCheck = false;
                    Vector2 pos;
                    if (Input.GetMouseButton(0))
                    {
                        if(Input.GetMouseButtonUp(0))
                            raycastCheck = true;
                        pos = Input.mousePosition;
                    }
                    else// if (Input.touchCount == 1)
                    {
                        if(Input.GetTouch(0).phase == TouchPhase.Ended)
                            raycastCheck = true;
                        pos = Input.GetTouch(0).position;
                    }

                    if (raycastCheck)
                    {
                        Ray ray = InputCamera.ScreenPointToRay(pos);
                        RaycastHit info = new RaycastHit();
                        if (Physics.Raycast(ray, out info, 10000))
                        {
                            EcosystemRaycastHandler(info.collider.gameObject.tag);
                        }
                    }
                    else if (!dragged)
                    {
                        //Scene rotation/scaling should be alternative to object picking and display
                        repositionTime = 0;
                        dragged = true;
                    }
                    else if (dragged)
                    {
                        Vector2 mouseDistance = pos - lastMouse;
                        Vector3 project =
                            new Vector3(mouseDistance.x, -mouseDistance.y, 0).normalized;// *MAX_ROT_SPEED;

                        Vector3 from = Guide.transform.forward;
                        Vector3 camY = Guide.transform.up;
                        Vector3 camX = -Guide.transform.right;
                        Vector3 to = (from + (camY * mouseDistance.y) + (camX * mouseDistance.x)).normalized;

                        Quaternion qrot = Quaternion.FromToRotation(from, to);
                        this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, qrot * this.transform.localRotation, Time.deltaTime * MAX_ROT_SPEED);
                    }
                    lastMouse = pos;
                }
            }
        }
        else
        {
            dragged = false;
            repositionTime += Time.deltaTime;
            if (repositionTime > REPOSITION_LIMIT)
            {
                this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.identity, Time.deltaTime * MAX_ROT_SPEED);
            }
        }
	}

    public void PauseScene()
    {
        ControlImage.sprite = play;
        ChildRecurse(false);
    }

    public void PlayScene()
    {
        ControlImage.sprite = pause;
        ChildRecurse(true);
    }

    public void ControlSimulation()
    {
        if (!paused)
        {
            PauseScene();
        }
        else
        {
            PlayScene();
        }
        paused = !paused;
    }

    private void ChildRecurse(bool enabled)
    {
        MonoBehaviour[] transforms = GetComponentsInChildren<MonoBehaviour>();
        if(transforms.Length > 0)
        {
            foreach (MonoBehaviour mono in transforms)
            {
                mono.enabled = enabled;
                Rigidbody rb = mono.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = !enabled;
                }
            }
        }
        this.enabled = true;
        return;
    }

    public virtual void EcosystemRaycastHandler(string objTag)
    {
        //Pause the simulation
        PauseScene();
        //Save the current camera scale and orientation
        
        //Preserve the content without tracking (disable Vuforia hierarchy/tracking somehow)

        //Check gameobject tag and attach 3D text label

        //Enable 'X' button to revert to AR tracking

        //Prevent screen-based scene interaction

        //Zoom camera to selected object
    }

    //How can we handle complex objects where parts exist in different tag layers? Don't make complex objects
    public void ToggleTagged(string tagLayer)
    {
        bool enabled = CheckLayerEnabled(tagLayer);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(tagLayer))
        {
            MonoBehaviour[] mono = go.GetComponents<MonoBehaviour>();
            if (mono != null)
            {
                foreach(MonoBehaviour mm in mono)
                    mm.enabled = enabled;
            }
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = enabled;
            }
            Renderer rr = go.GetComponent<Renderer>(); //Could be faster to try to modify camera layer, but since we're already looping objects...
            if (rr != null)
            {
                rr.enabled = enabled;
            }
        }
        //InputCamera.cullingMask = InputCamera.cullingMask & ((enabled ? 1 : 0) << LayerMask.NameToLayer(tagLayer));
    }

    public virtual bool CheckLayerEnabled(string layer)
    { return !this.enabled; }
}
