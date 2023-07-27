using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMapManager : MonoBehaviour
{
    [SerializeField] GameObject _keyMapUI;
    [SerializeField] KeyCode _showKey;
    [SerializeField] float _firstDuration = 10f;

    static bool _isFirst = true;

    private void Start()
    {
        if (_isFirst)
        {
            StartCoroutine(RemoveKeymap());
        }
    }
    IEnumerator RemoveKeymap()

    {
        yield return new WaitForSeconds(_firstDuration);
        _keyMapUI.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
        _keyMapUI.SetActive(false);
    }
    public void Update()
    {
        if (Input.GetKeyDown(_showKey))
        {
            _keyMapUI.SetActive(!_keyMapUI.activeInHierarchy);
            _isFirst = false;
        }
    }
}
