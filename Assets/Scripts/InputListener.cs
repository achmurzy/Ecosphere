using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class InputListener : MonoBehaviour {

    public struct TouchEvent    //We're saving the whole touch until this is shown to be a bad idea
    {
        public int count;
        public Vector2 pos;
        public TouchPhase touchType;
        public TouchEvent(int c, Vector2 p, TouchPhase tp) { count = c; pos = p; touchType = tp; }
    }

    void Awake()
    {
        Input.multiTouchEnabled = true;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}

    void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            Touch tt = Input.GetTouch(0);
            Dictionary<string, object> touchEvent = new Dictionary<string, object>();
            //TouchEvent te = new TouchEvent(1, tt.position);
            touchEvent.Add("touch", tt);
            Analytics.CustomEvent("Touch Event", touchEvent);
        }
    }
}
