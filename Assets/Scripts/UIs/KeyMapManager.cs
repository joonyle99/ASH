using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMapManager : MonoBehaviour
{
    [SerializeField] GameObject _keyMapUI;
    [SerializeField] KeyCode _showKey;
    public void Update()
    {
        if (Input.GetKeyDown(_showKey))
        {
            _keyMapUI.SetActive(!_keyMapUI.activeInHierarchy);
        }
    }
}
