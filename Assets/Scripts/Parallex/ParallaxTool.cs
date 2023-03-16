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
    [Tooltip("각 레이어 별 최소 z 값을 지정합니다. \n마지막 레이어는 이전 레이어의 최소 z값보다 작은 모든 이미지를 포함합니다.")]
    [SerializeField] ParallaxBoundaries _parallaxBoundaries= new ParallaxBoundaries();

    [Header("Volumetric Light")]
    [SerializeField][Tooltip("안개 효과 색")] Color _volumetricLightColor = Color.white;
    [SerializeField][Tooltip("안개 효과 강도")][Range(0f, 1f)] float _volumeOpacity = 1f;

    [Tooltip("안개효과 적용 방식\n DistanceBased : 거리를 기준으로 투명도 설정\n LayerBased : 같은 안개 이미지를 겹겹이 쌓아서 생성")]
    [SerializeField] VolumetricLightType _volumeType = VolumetricLightType.DistanceBased;

    [Tooltip("Distance Based 방식 사용 시 월드의 최대 깊이")]
    [SerializeField]float _volumeDepth = 200f;

    [Header("Global Light")]
    [Tooltip("월드 전체에 적용 될 빛의 색")]
    [SerializeField] Color _globalLightColor = Color.white;
    [Tooltip("월드 전체에 적용 될 빛의 세기")]
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
            //TODO : 안개효과 이미지가 카메라를 쫓아가고 사이즈가 카메라에 딱맞게 만들기? 를 해야할지도
            _volumetricLights[i].transform.position = new Vector3(0, 0, z);
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
        if (GUILayout.Button(new GUIContent("Sort Layers", "Parallax Boundaries 설정에 맞게 모든 이미지를 정렬")))
        {
            tool.SortLayers();
        }
    }
}
#endif