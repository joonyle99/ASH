using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Rect _frameBoundary;
    [SerializeField] float _followSpeed;

    private void Update()
    {/*
        Rect frameRect = _frameBoundary;
        frameRect.position += new Vector2(transform.position.x, transform.position.y);
        frameRect.position -= frameRect.size / 2;

        //Keep player in boundary
        Vector3 pointAtBorder = Vector3.zero;
        pointAtBorder.x = Mathf.Clamp(_target.position.x, frameRect.xMin, frameRect.xMax);
        pointAtBorder.y = Mathf.Clamp(_target.position.y, frameRect.yMin, frameRect.yMax);

        Vector3 moveVec = _target.position - pointAtBorder;
        transform.position += moveVec;
        */
        
        //transform.position = Vector3.MoveTowards(transform.position, _target.position, _followSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, _target.position, _followSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(_frameBoundary.width, _frameBoundary.height, 1));
    }
}
