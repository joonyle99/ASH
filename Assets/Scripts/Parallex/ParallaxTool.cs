using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParallaxTool : MonoBehaviour
{
    [SerializeField] ParallaxBoundaries _parallaxBoundaries= new ParallaxBoundaries();
    
    List<ParallaxLayer> _parallaxLayers;
    List<SpriteRenderer> _registeredSprites = new List<SpriteRenderer>();
    public void SortLayers()
    {
        var allSprites = GameObject.FindObjectsOfType<SpriteRenderer>();
        //TODO : Parallax 영향 안받을 오브젝트들에 대한 처리 방법
        //
        for (int i=0; i<allSprites.Length; i++)
        {
            allSprites[i].sortingLayerName = _parallaxBoundaries.GetLayerName(allSprites[i].transform.position.z);
            allSprites[i].sortingOrder = -(int)(allSprites[i].transform.position.z * 100);
        }
    }
    private void OnValidate()
    {
        _parallaxBoundaries.OnValidate();
    }
    private void Reset()
    {
        _parallaxBoundaries.SetSortingLayers(SortingLayer.layers);
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