using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxEditorSupport : MonoBehaviour
{
    [SerializeField][Tooltip("z축을 움직이면 자동으로 sorting layer도 조정됩니다.")]
    bool _autoSort = true;
    SpriteRenderer _spriteRenderer;
    ParallaxTool _parallaxTool;

    [SerializeField][Tooltip("z축을 움직여도 scale이 고정됩니다.")]
    bool _autoScale = true;
    Vector3 _baseScale;
    //Set sorting layer order only on editor

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseScale = transform.localScale;
    }
    private void Update()
    {
        if (!Selection.Contains(gameObject))
            return;
        if (_autoSort)
        {
            if (_parallaxTool == null)
                _parallaxTool = FindObjectOfType<ParallaxTool>();
            _spriteRenderer.sortingOrder = ParallaxTool.GetSortingOrder(transform.position.z);
            _spriteRenderer.sortingLayerName = _parallaxTool.GetSortingLayerName(transform.position.z);
        }
        if(_autoScale)
        {
            float zOffset = transform.position.z - Camera.main.transform.position.z;
            transform.localScale = _baseScale * zOffset / (0 - Camera.main.transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
        }
        else
        {
            float zOffset = transform.position.z - Camera.main.transform.position.z;

            _baseScale = transform.localScale / zOffset * (0 - Camera.main.transform.position.z);
            _baseScale.z = 1;
        }
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

