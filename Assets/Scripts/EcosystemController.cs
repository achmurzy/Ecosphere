using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PauseScene()
    {
        ChildRecurse(false);
        
    }

    public void PlayScene()
    {
        ChildRecurse(true);
    }

    private void ChildRecurse(bool enabled)
    {
        MonoBehaviour[] transforms = GetComponentsInChildren<MonoBehaviour>();
        if(transforms.Length > 0)
        {
            foreach (MonoBehaviour mono in transforms)
            {
                mono.enabled = enabled;
            }
        }
        return;
    }

}
