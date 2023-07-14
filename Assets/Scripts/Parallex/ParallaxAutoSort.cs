using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxAutoSort : MonoBehaviour
{
    [SerializeField] bool _autoSort = true;
    SpriteRenderer _spriteRenderer;
    ParallaxTool _parallaxTool;
    //Set sorting layer order only on editor

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        print(_parallaxTool);
    }
    private void Update()
    {
        if (!Selection.Contains(gameObject))
            return;
        if (!_autoSort)
            return;
        if (_parallaxTool == null)
            _parallaxTool = FindObjectOfType<ParallaxTool>();
        _spriteRenderer.sortingOrder = ParallaxTool.GetSortingOrder(transform.position.z);
        _spriteRenderer.sortingLayerName = _parallaxTool.GetSortingLayerName(transform.position.z);
    }
    /*
    private void OnDrawGizmosSelected()
    {
        if (!_autoScale)
            return;
        float zOffset = transform.position.z - Camera.main.transform.position.z;
        transform.localScale = new Vector3(zOffset, zOffset, 0) / (-Camera.main.transform.position.z);
        //print(gameObject.name + " " +  zOffset.ToString());
    }*/
}

