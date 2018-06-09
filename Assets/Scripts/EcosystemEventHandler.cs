using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Vuforia;

class EcosystemEventHandler : DefaultTrackableEventHandler
{
    public ARController arController;
    private Dictionary<string, object> eventObject;

    private void Awake()
    {
        eventObject = new Dictionary<string, object>();
        eventObject.Add("Name", this.gameObject.name); 
    }

    protected override void OnTrackingFound()
    {
        Analytics.CustomEvent("Trackable Found", eventObject);
        arController.LoadScene(this.gameObject);
        /*var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;*/
    }


    protected override void OnTrackingLost()
    {
        Analytics.CustomEvent("Trackable Lost", eventObject);
        GameObject.Destroy(this.transform.GetChild(0).gameObject);
        /*var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;*/
    }
}
