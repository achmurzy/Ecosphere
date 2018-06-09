using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARController : MonoBehaviour
{
    private EcosystemController ecosystem;
    public bool Paused { get; private set; }
    private Sprite pause, play;
    public Image ControlImage;

    void Awake()
    {
        Paused = false;
        pause = Resources.Load("Images/pause", typeof(Sprite)) as Sprite;
        play = Resources.Load("Images/play", typeof(Sprite)) as Sprite;
    }

    public void LoadScene(GameObject target)
    {
        Paused = false;
        GameObject scene = Resources.Load("Prefabs/Scenes/" + target.name) as GameObject;
        scene.transform.parent = target.transform;
        scene.transform.localPosition = Vector3.zero;
        ecosystem = scene.GetComponent<EcosystemController>();
    }

    public void ControlSimulation()
    {
        if (!Paused)
        {
            //ecosystem.PauseScene();
            ControlImage.sprite = play;
        }
        else
        {
            //ecosystem.PlayScene();
            ControlImage.sprite = pause;
        }
        Paused = !Paused;
    }
}
