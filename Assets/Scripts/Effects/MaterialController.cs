using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MaterialController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _spriteRenderers;
    private Material[] _instanceMaterials;

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

    // public Material MainMaterial => SpriteRenderers[0].material;

    private void Awake()
    {
        // save original Materials
        _instanceMaterials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _instanceMaterials.Length; i++)
            _instanceMaterials[i] = _spriteRenderers[i].material;

        // effects
        BlinkEffect = GetComponent<BlinkEffect>();
        DisintegrateEffect = GetComponent<DisintegrateEffect>();
    }

    public void CollectAllSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    public void InitMaterial()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _instanceMaterials[i];
    }
    public void InitMaterialForRespawn()
    {
        DisintegrateEffect.InitMaterialAndProgressForRespawn();
    }

    public void SetMaterial(Material material)
    {
        foreach (var spriteRenderer in _spriteRenderers)
            spriteRenderer.material = material;
    }
    public void SetProgress(string key, float progress)
    {
        foreach (var spriteRenderer in _spriteRenderers)
            spriteRenderer.material.SetFloat(key, progress);
    }
    public void SetMaterialAndProgress(Material material, string key, float progress)
    {
        foreach (var spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.material = material;
            spriteRenderer.material.SetFloat(key, progress);
        }
    }

    public void EnableGodModeOutline()
    {
        //Debug.Log("Enable GodMode Outline");
        foreach (var material in _instanceMaterials)
        {
            material.EnableKeyword("OUTBASE_ON");
        }
    }
    public void DisableGodModeOutline()
    {
        //Debug.Log("Disable GodMode Outline");
        foreach (var material in _instanceMaterials)
        {
            material.DisableKeyword("OUTBASE_ON");
        }
    }
    public void EnableHitEffect()
    {
        foreach (var material in _instanceMaterials)
        {
            material.EnableKeyword("HITEFFECT_ON");
        }
    }
    public void DisableHitEffect()
    {
        foreach (var material in _instanceMaterials)
        {
            material.DisableKeyword("HITEFFECT_ON");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MaterialController))]
public class MaterialControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MaterialController materialController = (MaterialController)target;

        if (GUILayout.Button("Get All SpriteRenderer in Children"))
        {
            materialController.CollectAllSpriteRenderers();
            EditorUtility.SetDirty(materialController);
        }
    }
}
#endif