using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluxRibbon : MonoBehaviour {

    public bool Wiggle = false;
    public float Amplitude = 1f;
    public float Frequency = 1f;

    public Vector3 wiggleAxis1 = Vector3.up;
    public Vector3 wiggleAxis2 = Vector3.down;
    
    private float wiggleLerp = 0f;
    private float lerpSign = 1;

    private Emitter fluxer;
    private Vector3 initTraj;
    private Vector3 properUp;
    
    public Bezier3D bezier;
    private Vector3 bStart, bEnd, bHandle1, bHandle2;

    void Awake()
    {
        bezier = GetComponentInChildren<Bezier3D>();
        bStart = bezier.Start;
        bEnd = bezier.End;
        bHandle1 = bezier.Handle1;
        bHandle2 = bezier.Handle2;
        bezier.UpNormal = (transform.position - Camera.main.transform.position).normalized;
    }
    
    // Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        //For Beziers along the x-axis and controls along the y-axis, the z-axis must point toward the camera
        bezier.UpNormal = (transform.position - Camera.main.transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(initTraj, bezier.UpNormal);
        transform.position += transform.forward.normalized * fluxer.EmissionForce;
        if (Wiggle)
        {
            wiggleLerp += Time.deltaTime * Frequency * Mathf.Sign(lerpSign);
            wiggleLerp = Mathf.Clamp(wiggleLerp, 0, 1);

            bezier.Handle1 = Vector3.Lerp(bHandle1 - wiggleAxis1, bHandle1 + wiggleAxis1, wiggleLerp) * Amplitude;
            bezier.Handle2 = Vector3.Lerp(bHandle2 - wiggleAxis2, bHandle2 + wiggleAxis2, wiggleLerp) * Amplitude;

            if (wiggleLerp == 1 || wiggleLerp == 0)
            {
                lerpSign = lerpSign * -1;
            }
        }
	}

    public void StartFluxing(Vector3 target, Emitter flux)
    {
        initTraj = target;
        fluxer = flux;
    }

    public void TriggerExit(Collider other)
    {
        if (other.transform == fluxer.transform)
        {
            Debug.Log("Fluxer destroyed by exit: " + Vector3.Distance(this.transform.position, Vector3.zero));
            GameObject.Destroy(transform.GetChild(0).gameObject);
            GameObject.Destroy(this);
        }
    }
}
