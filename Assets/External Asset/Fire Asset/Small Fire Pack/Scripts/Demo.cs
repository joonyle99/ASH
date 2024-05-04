using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

  
    public GameObject[] FXList;
    public Text Title;
    public int Selection = 0;

    public GameObject BackText;
    public GameObject NextText;
    public GameObject BackButton;
    public GameObject NextButton;

    void Start () {
        FXList[Selection].SetActive(true);
        Title.text = FXList[Selection].gameObject.transform.name.ToString();
    }
	void Update()
    {
        if(Selection == 0)
        {
            BackText.SetActive(false);
            BackButton.SetActive(false);
        }
        else
        {
            BackText.SetActive(true);
            BackButton.SetActive(true);
        }

        if(Selection == FXList.Length - 1)
        {
            NextText.SetActive(false);
            NextButton.SetActive(false);
        }
        else
        {
            NextText.SetActive(true);
            NextButton.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Back();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Next();
        }


    }
    public void Back()
    {
       
        if (Selection < FXList.Length && Selection != 0)
        {
            FXList[Selection].SetActive(false);
            Selection -= 1;
            FXList[Selection].SetActive(true);
            Title.text = FXList[Selection].gameObject.transform.name.ToString();
        }
        
    }

    public void Next()
    {
       
        if(Selection < FXList.Length && Selection != FXList.Length - 1)
        {
            FXList[Selection].SetActive(false);
            Selection += 1;
            FXList[Selection].SetActive(true);
            Title.text = FXList[Selection].gameObject.transform.name.ToString();
        }
       
    }
}
