using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMapManager : MonoBehaviour
{
    [SerializeField] GameObject _keyMapUI;
    [SerializeField] KeyCode _showKey;
    [SerializeField] float _firstDuration = 10f;

    [SerializeField] bool _isFirst = false;

    private void Start()
    {
        if (_isFirst && _keyMapUI != null)
        {
            StartCoroutine(RemoveKeymap());
        }
    }
    IEnumerator RemoveKeymap()
    {
        float eTime = 0f;
        while(eTime < _firstDuration)
        {
            eTime += Time.deltaTime;
            if (!_keyMapUI.activeInHierarchy)
                yield break;
            yield return null;
        }
        if(_keyMapUI.activeInHierarchy)
        {
            _keyMapUI.GetComponent<Animator>().SetTrigger("Close");
            yield return new WaitForSeconds(0.5f);
            _keyMapUI.SetActive(false);
        }
    }
    public void Update()
    {
        /*
        if (Input.GetKeyDown(_showKey))
        {
            _keyMapUI.SetActive(!_keyMapUI.activeInHierarchy);
            _isFirst = false;
        }
        */
    }
}
