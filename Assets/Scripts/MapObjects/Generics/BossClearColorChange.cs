using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossClearColorChange : MonoBehaviour
{
    [SerializeField] Sprite _changedSprite;

    SpriteRenderer _renderer;
    public void Initialize(Material material)
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.material = material;
        _renderer.material.SetTexture("_ChangeTex", _changedSprite.texture);
    }

    public void SetProgress(float progress)
    {
        _renderer.material.SetFloat("_Progress", progress);
    }
}
