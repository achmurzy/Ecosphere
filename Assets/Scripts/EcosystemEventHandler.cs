using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Vuforia;

public class EcosystemEventHandler : DefaultTrackableEventHandler
{
    public ARController arController;
    public GameObject Ecosystem;
    public TrackableBehaviour.Status State;

    private Dictionary<string, object> eventObject;

    private void Awake()
    {
        eventObject = new Dictionary<string, object>();
        eventObject.Add("Name", this.gameObject.name); 
    }

    protected override void OnTrackingFound()
    {
        Analytics.CustomEvent("Trackable Found", eventObject);

        if (arController.Trackable == this)
            Ecosystem.GetComponent<EcosystemController>().PlayScene();
        else
            Ecosystem = arController.LoadScene(this);    //Destroys last ecosystem

        var rendererComponents = GetComponentsInChildren<Renderer>(true);
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
            component.enabled = true;
    }


    protected override void OnTrackingLost()
    {
        Analytics.CustomEvent("Trackable Lost", eventObject);

        /*if (Ecosystem != null)
        {
            Ecosystem.GetComponent<EcosystemController>().PauseScene();
        }

        //Do not require TRACKABLE_FOUND to show ecosystem
          Need a camera rig to accomodate not having an anchor in the scene
         *  If there is no trackable, what will the ecosystem do within the scene?
         *  Needs testingS
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
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
            component.enabled = false;
         * */
    }

    /*public override void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        State = newStatus;
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }*/
}
