using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParallaxTool : MonoBehaviour
{
    public enum VolumetricLightType { DistanceBased, LayerBased}

    [SerializeField] private GameObject _backgroundParent;

    [Space]

    [Tooltip("�� ���̾� �� �ּ� z ���� �����մϴ�.\n������ ���̾�� ���� ���̾��� �ּ� z������ ���� ��� �̹����� �����մϴ�.")]
    [SerializeField] private ParallaxBoundaries _parallaxBoundaries = new ParallaxBoundaries();

    [Header("Volumetric Light")]
    [Space]
    
    [Tooltip("�Ȱ� ȿ�� ��")]
    [SerializeField] private Color _volumetricLightColor = Color.white;
    [Tooltip("�Ȱ� ȿ�� ����")] [Range(0f, 1f)]
    [SerializeField] private float _volumeOpacity = 1f;
    [Tooltip("�Ȱ�ȿ�� ���� ���\n DistanceBased : �Ÿ��� �������� ���� ����\n LayerBased : ���� �Ȱ� �̹����� ����� �׾Ƽ� ����")]
    [SerializeField] private VolumetricLightType _volumeType = VolumetricLightType.DistanceBased;

    [Space]

    [Tooltip("Distance Based ��� ��� �� ������ �ִ� ����")]
    [SerializeField] private float _volumeDepth = 200f;

    [Header("Global Light")]
    [Space]

    [Tooltip("���� ��ü�� ���� �� ���� ��")]
    [SerializeField] private Color _globalLightColor = Color.white;
    [Tooltip("���� ��ü�� ���� �� ���� ����")]
    [SerializeField] private float _globalLightIntensity = 1f;

    private SpriteRenderer[] _volumetricLights;
    private Light2D _globalLight;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorUtility.IsPersistent(gameObject))
            return;
        _parallaxBoundaries.OnValidate();
        SetVolumetricLights();
        SetGlobalLight();
    }
#endif

    private void Reset()
    {
        _parallaxBoundaries.SetSortingLayers(SortingLayer.layers);
    }

    public void SortLayers()
    {
        SetVolumetricLights();
        SetGlobalLight();
        var allSprites = _backgroundParent.transform.GetComponentsInChildren<SpriteRenderer>();
        // TODO : Parallax ���� �ȹ��� ������Ʈ�鿡 ���� ó�� ���
        //
        for (int i = 0; i < allSprites.Length; i++)
        {
            if (allSprites[i].transform.parent == transform)
            {
                continue;
            }
            allSprites[i].sortingLayerName = _parallaxBoundaries.GetLayerName(allSprites[i].transform.position.z);
            allSprites[i].sortingOrder = GetSortingOrder(allSprites[i].transform.position.z);
        }


    }
    public void SetGlobalLight()
    {
        _globalLight = GetComponentInChildren<Light2D>();
        _globalLight.intensity = _globalLightIntensity;
        _globalLight.color = _globalLightColor;
    }
    public void SetVolumetricLights()
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
            // TODO : �Ȱ�ȿ�� �̹����� ī�޶� �Ѿư��� ����� ī�޶� ���°� �����? �� �ؾ�������
            _volumetricLights[i].transform.position = new Vector3(0, 0, z);
            _volumetricLights[i].color = color;
            _volumetricLights[i].sortingLayerName = _parallaxBoundaries.GetEnabledLayerNames()[i];
            _volumetricLights[i].sortingOrder = GetSortingOrder(minBoundaries[i - 1]);

        }
        if (Camera.main != null && _volumeType == VolumetricLightType.DistanceBased)
            Camera.main.backgroundColor = _volumetricLightColor * _volumeOpacity;
    }
    public static int GetSortingOrder(float z)
    {
        return -(int)(z * 100);
    }
    public string GetSortingLayerName(float z)
    {
        return _parallaxBoundaries.GetLayerName(z);
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
        if (GUILayout.Button(new GUIContent("Sort Layers", "Parallax Boundaries ������ �°� ��� �̹����� ����")))
        {
            tool.SortLayers();
        }
    }
}
#endif