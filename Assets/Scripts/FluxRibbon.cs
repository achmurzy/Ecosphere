using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluxRibbon : MonoBehaviour {

    public bool wiggle = true;
    public float frequency = 1f;
    public float amplitude = 1f;

    public Vector3 wiggleAxis1 = Vector3.up;
    public Vector3 wiggleAxis2 = Vector3.down;
    
    private float wiggleLerp = 0f;
    private float lerpSign = 1;
    
    public Bezier3D bezier;
    private Vector3 bStart, bEnd, bHandle1, bHandle2;

    private FluxEmitter emitter;
    private Vector3 origPos;

    void Awake()
    {
        bezier = GetComponentInChildren<Bezier3D>();
        bStart = bezier.start;
        bEnd = bezier.end;
        bHandle1 = bezier.handle1;
        bHandle2 = bezier.handle2;
    }
    
    // Use this for initialization
	void Start () 
    {
        origPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (wiggle)
        {
            wiggleLerp += Time.deltaTime * frequency * Mathf.Sign(lerpSign);
            wiggleLerp = Mathf.Clamp(wiggleLerp, -1, 1);
            float y = Mathf.Sin(wiggleLerp) * amplitude;

            bezier.handle1 = new Vector3(bHandle1.x, y, 0);
            bezier.handle2 = new Vector3(bHandle2.x, -y, 0);

            if (wiggleLerp == 1 || wiggleLerp == -1)
            {
                lerpSign = lerpSign * -1;

            }

            bezier.GetComponent<MeshFilter>().mesh = bezier.CreateMesh();
        }
        else
        {
            wiggleLerp += Time.deltaTime * frequency;
            if (wiggleLerp >= 1f)
                GameObject.Destroy(this.gameObject);
            this.transform.position = Vector3.Lerp(origPos, 
                origPos + (emitter.transform.forward * emitter.DestructionDistance), wiggleLerp);    
        }
	}
}
