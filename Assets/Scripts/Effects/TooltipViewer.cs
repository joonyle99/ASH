using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipViewer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject)
        {
            PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
            if(player != null)
            {
                Debug.Log("Trigger tooltip viewer");
                GameUIManager.OpenDoubleJumpSkillObtainPanel();
            }
        }
    }
}
