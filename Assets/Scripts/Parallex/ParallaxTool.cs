using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParallaxTool : MonoBehaviour
{
    public enum VolumetricLightType { DistanceBased, LayerBased}
    [SerializeField] ParallaxBoundaries _parallaxBoundaries= new ParallaxBoundaries();
    [Header("Volumetric Light")]
    [SerializeField] Color _volumetricLightColor = Color.white;
    [SerializeField][Range(0f, 1f)] float _volumeOpacity = 1f;
    [SerializeField]float _volumeDepth = 200f;
    [SerializeField] VolumetricLightType _volumeType = VolumetricLightType.DistanceBased;

    [Header("Global Light")]
    [SerializeField] Color _globalLightColor = Color.white;
    [SerializeField] float _globalLightIntensity = 1f;

    SpriteRenderer[] _volumetricLights;
    Light2D _globalLight;
    public void SortLayers()
    {
        SetVolumetricLights();
        SetGlobalLight();
        var allSprites = FindObjectsOfType<SpriteRenderer>();
        //TODO : Parallax 영향 안받을 오브젝트들에 대한 처리 방법
        //
        for (int i=0; i<allSprites.Length; i++)
        {
            if (allSprites[i].transform.parent == transform)
            {
                continue;
            }
            allSprites[i].sortingLayerName = _parallaxBoundaries.GetLayerName(allSprites[i].transform.position.z);
            allSprites[i].sortingOrder = GetSortingOrder(allSprites[i].transform.position.z);
        }


    }
    private void OnValidate()
    {
        _parallaxBoundaries.OnValidate();
        SetVolumetricLights();
        SetGlobalLight();
    }
    private void Reset()
    {
        _parallaxBoundaries.SetSortingLayers(SortingLayer.layers);
    }
    void SetGlobalLight()
    {
        _globalLight = GetComponentInChildren<Light2D>();
        _globalLight.intensity = _globalLightIntensity;
        _globalLight.color = _globalLightColor;
    }
    void SetVolumetricLights()
    {
        _volumetricLights = GetComponentsInChildren<SpriteRenderer>(true);
        var minBoundaries = _parallaxBoundaries.GetEnabledMinBoundaries();
        for (int i = 0; i < _volumetricLights.Length; i++)
        {
            if (i >= minBoundaries.Count || i == 0)
            {
                _volumetricLights[i].gameObject.SetActive(false);
                continue;
            }
            _volumetricLights[i].gameObject.SetActive(true);

            float z = minBoundaries[i - 1];
            Color color = _volumetricLightColor;
            if (_volumeType == VolumetricLightType.LayerBased)
                color.a = _volumeOpacity;
            else
            {
                color.a = _volumeOpacity * (z / _volumeDepth);
                for (int j = 0; j < i; j++)
                    _volumetricLights[j].color = new Color(_volumetricLights[j].color.r, _volumetricLights[j].color.g, _volumetricLights[j].color.b,
                                                    _volumetricLights[j].color.a - color.a);
            }
            _volumetricLights[i].transform.position = new Vector3(_volumetricLights[i].transform.position.x, _volumetricLights[i].transform.position.y, z);
            _volumetricLights[i].color = color;
            _volumetricLights[i].sortingLayerName = _parallaxBoundaries.GetEnabledLayerNames()[i];
            _volumetricLights[i].sortingOrder = GetSortingOrder(minBoundaries[i - 1]);
        }
        if (_volumeType == VolumetricLightType.DistanceBased)
            Camera.main.backgroundColor = _volumetricLightColor * _volumeOpacity;
    }
    int GetSortingOrder(float z)
    {
        return -(int)(z * 100);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ParallaxTool))]
public class ParallaxToolInspector: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ParallaxTool tool = (ParallaxTool)target;
        if (GUILayout.Button("Sort Layers"))
        {
            tool.SortLayers();
        }
    }
}
#endif