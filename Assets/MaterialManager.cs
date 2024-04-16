using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [field: SerializeField]
    public SpriteRenderer[] SpriteRenderers
    {
        get;
        private set;
    }
    public Material[] OriginalMaterials
    {
        get;
        private set;
    }

    public BlinkEffect BlinkEffect
    {
        get;
        private set;
    }
    public DisintegrateEffect DisintegrateEffect
    {
        get;
        private set;
    }
    public RespawnEffect RespawnEffect
    {
        get;
        private set;
    }

    private void Awake()
    {
        // effects
        BlinkEffect = GetComponent<BlinkEffect>();
        DisintegrateEffect = GetComponent<DisintegrateEffect>();
        RespawnEffect = GetComponent<RespawnEffect>();

        // save original Materials
        OriginalMaterials = new Material[SpriteRenderers.Length];
        for (int i = 0; i < OriginalMaterials.Length; i++)
            OriginalMaterials[i] = SpriteRenderers[i].material;
    }

    public void InitMaterial()
    {
        for (int i = 0; i < SpriteRenderers.Length; i++)
            SpriteRenderers[i].material = OriginalMaterials[i];
    }
    public void SetMaterial(Material material)
    {
        foreach (var spriteRenderer in SpriteRenderers)
            spriteRenderer.material = material;
    }
    public void SetProgress(string key, float progress)
    {
        foreach (var spriteRenderer in SpriteRenderers)
            spriteRenderer.material.SetFloat(key, progress);
    }
    public void SetMaterialAndProgress(Material material, string key, float progress)
    {
        foreach (var spriteRenderer in SpriteRenderers)
        {
            spriteRenderer.material = material;
            spriteRenderer.material.SetFloat(key, progress);
        }
    }
}
