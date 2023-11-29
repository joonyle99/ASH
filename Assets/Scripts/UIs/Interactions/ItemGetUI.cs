using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGetUI : MonoBehaviour
{
    GameObject Item_img, UI_main, UI_sub;
    // Start is called before the first frame update
    void Start()
    {
        Item_img = transform.GetChild(0).gameObject;
        UI_main = transform.GetChild(1).gameObject;
        UI_sub = transform.GetChild(2).gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
