using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDrop : MonoBehaviour {

    float rainLerp = 0;
    public const float RAIN_SPEED = 1f;
    public const float RAIN_WIDTH = 0.25f, RAIN_LENGTH = 0.46f;

    public Cloud Nimbus;
    public Vector3 origPos, goalPos, origScale, goalScale;
    public float cloudHeight;

    bool transmit = true;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rainLerp += Time.deltaTime * RAIN_SPEED;
        if (rainLerp >= 1f)
            GameObject.Destroy(this.gameObject);

        if (!transmit)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, rainLerp);
        }
        else
        {
            this.transform.localPosition = Vector3.Lerp(origPos, goalPos, rainLerp);
            this.transform.localScale = Vector3.Lerp(origScale, goalScale, rainLerp);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Crown photo = other.GetComponent<Crown>();
        if (photo != null && transmit)
        {
            photo.Rainfall();

        }
    }
}
