using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePieceGet : MonoBehaviour
{
    GameObject ItemGetUI;
    // Start is called before the first frame update
    void Start()
    {
        ItemGetUI = GameObject.Find("Canvas").transform.Find("opaque_ui").gameObject;
    }

    void activateItemGetUIText() {
        
        ItemGetUI.transform.GetChild(1).gameObject.SetActive(true);
        ItemGetUI.transform.GetChild(2).gameObject.SetActive(true);
    }

    void deactivateItemGetUI() {
        ItemGetUI.SetActive(false);
        ItemGetUI.transform.GetChild(0).gameObject.SetActive(false);
        ItemGetUI.transform.GetChild(1).gameObject.SetActive(false);
        ItemGetUI.transform.GetChild(2).gameObject.SetActive(false);
    }

    void activateSkillGetUIText()
    {

        ItemGetUI.transform.GetChild(3).gameObject.SetActive(true);
    }

    void deactivateSkillGetUI()
    {
        ItemGetUI.SetActive(false);
        ItemGetUI.transform.GetChild(3).gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            
            ItemGetUI.SetActive(true);
            if (gameObject.name == "skill_test")
            {
                Debug.Log("Ω∫≈≥ »πµÊ UI »£√‚");
                Invoke("activateSkillGetUIText", 0f);
                Invoke("deactivateSkillGetUI", 5f);
            }
            else {
                ItemGetUI.transform.GetChild(0).gameObject.SetActive(true);
                Debug.Log("æ∆¿Ã≈€ »πµÊ UI »£√‚");
                Invoke("activateItemGetUIText", 1f);
                Invoke("deactivateItemGetUI", 5f);
            }
            gameObject.SetActive(false);
            

        }
        else {
            Debug.Log("√Êµπ¿∫ «‘, ¿Ã∞‘ ππ¿”");
        }
    }
}
