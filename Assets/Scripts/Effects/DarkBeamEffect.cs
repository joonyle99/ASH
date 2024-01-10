using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBeamEffect : MonoBehaviour
{
    [SerializeField] float _segmentLength;
    [SerializeField] float _thickness;

    [SerializeField] SortingLayer _sortingLayer;
    [SerializeField] int _orderInLayer;

    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();   
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer.sortingLayerID = _sortingLayer.id;
        _meshRenderer.sortingOrder = _orderInLayer;
    }
    public void RecreateMesh(float length)
    {
        int segmentCount = Mathf.CeilToInt(length / _segmentLength);
        Vector3 direction = Vector3.right;
        Mesh mesh = new Mesh();

        int vertexCount = 4 * segmentCount;
        int triangleCount = 2 * segmentCount;

        Vector3[] verticies = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[3 * triangleCount];

        for(int i=0; i< segmentCount; i++)
        {
            float currentSegmentLength = i < segmentCount - 1 ? _segmentLength : length - _segmentLength * i;
            Vector3 start = direction * _segmentLength * i;
            Vector3 end = start + direction * currentSegmentLength;

            verticies[i * 4 + 0] = start - new Vector3(0, _thickness / 2, 0);
            verticies[i * 4 + 1] = start + new Vector3(0, _thickness / 2, 0);
            verticies[i * 4 + 2] = end + new Vector3(0, _thickness / 2, 0);
            verticies[i * 4 + 3] = end - new Vector3(0, _thickness / 2, 0);

            uvs[i * 4 + 0] = new Vector2(0, 0);
            uvs[i * 4 + 1] = new Vector2(0, 1);
            uvs[i * 4 + 2] = new Vector2(currentSegmentLength / _segmentLength, 1);
            uvs[i * 4 + 3] = new Vector2(currentSegmentLength / _segmentLength, 0);

            triangles[i * 6 + 0] = i * 4 + 0;
            triangles[i * 6 + 1] = i * 4 + 1;
            triangles[i * 6 + 2] = i * 4 + 2;

            triangles[i * 6 + 3] = i * 4 + 2;
            triangles[i * 6 + 4] = i * 4 + 3;
            triangles[i * 6 + 5] = i * 4 + 0;
        }
        mesh.vertices = verticies;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        _meshFilter.mesh = mesh;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float rot = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(rot), Mathf.Sin(rot)) * 3);
    }
}
