using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : InteractableObject
{
    [SerializeField] GameObject _passage;
    // Start is called before the first frame update
    
    protected override void OnInteract()
    {
        if (PersistentDataManager.Get<bool>(BossDungeonManager.Instance.DataGroupName, BossDungeonManager.Instance.DoorDataID))
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("¿­¼è¸¦ ´ú ¸ð¾Ò¾î¿ä");
        }
        ExitInteraction();
    }
    void OpenDoor()
    {
        _passage.SetActive(true);
    }
}
