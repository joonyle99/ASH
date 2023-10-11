using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : ITriggerZone
{
    [SerializeField] Transform _spawnPoint;
#if UNITY_EDITOR
    PolygonCollider2D __polyconCollider;
#endif
    public Vector3 SpawnPosition { get { return _spawnPoint.position; } }

    public override void OnActivatorEnter(TriggerActivator activator) 
    {
        SceneContext.Current.CheckPointManager.OnPlayPassedCheckpoint(this);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 1);
        if (_spawnPoint != null)
        {
            Gizmos.DrawSphere(_spawnPoint.position, 0.25f);
            Gizmos.DrawLine(transform.position, _spawnPoint.position);
            Vector2[] points = (__polyconCollider == null ? __polyconCollider = GetComponent<PolygonCollider2D>() : __polyconCollider).points;
            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                Gizmos.DrawLine(transform.position + new Vector3(points[i].x, points[i].y, 0),
                    transform.position + new Vector3(points[j].x, points[j].y, 0));
            }
        }
        //Gizmos.color = Color.green;
        float lineLength = 0.3f;
        Gizmos.DrawLine(transform.position - new Vector3(lineLength, lineLength, 0), transform.position + new Vector3(lineLength, lineLength, 0));
        Gizmos.DrawLine(transform.position - new Vector3(-lineLength, lineLength, 0), transform.position + new Vector3(-lineLength, lineLength, 0));

    }

}
