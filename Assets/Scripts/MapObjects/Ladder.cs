using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] Transform _upperPart;
    [SerializeField] Transform _lowerPart;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float height = spriteRenderer.size.y;
    }
}
