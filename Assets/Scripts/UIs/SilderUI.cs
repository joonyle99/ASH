using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SilderUI : MonoBehaviour
{ 
    public Slider _slider;
    public TextMeshProUGUI sliderText;

    private void Update()
    {
        sliderText.text = _slider.value.ToString();
    }
}
