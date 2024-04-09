using UnityEngine;

public class BossClearColorChange : MonoBehaviour
{
    public enum ChangeType
    {
        Material,
        OverlayAlpha
    }

    [SerializeField] ChangeType _changeType;
    [SerializeField] Sprite _changedSprite;

    SpriteRenderer _renderer;

    public void Initialize(Material material)
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_changeType == ChangeType.Material)
        {
            _renderer.material = material;
            _renderer.material.SetTexture("_ChangeTex", _changedSprite.texture);
            _renderer.material.SetFloat("_Progress", 0f);
        }
        else
        {
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0);
        }
    }

    public void SetProgress(float progress)
    {
        if (_changeType == ChangeType.Material)
            _renderer.material.SetFloat("_Progress", progress);
        else
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, progress);
    }
}
