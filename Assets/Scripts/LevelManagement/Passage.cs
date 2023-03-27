using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour, ITriggerZone
{
    [SerializeField] Tymski.SceneReference targetScene;


    public void OnActivatorEnter(TriggerActivator activator)
    {
        SceneTransitionManager.Instance.ChangeScene(targetScene);
    }
}