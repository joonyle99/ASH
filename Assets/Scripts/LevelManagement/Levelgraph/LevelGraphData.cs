using System;
using UnityEngine;
using System.Collections.Generic;


namespace LevelGraph
{
    [CreateAssetMenu(fileName = "LevelGraphData", menuName = "LevelManagement/Level Graph Data")]
    public class LevelGraphData : ScriptableObject
    {
        [SerializeField] List<SceneData> _nodes = new List<SceneData>();
        [SerializeField] List<PassagePairData> _edges = new List<PassagePairData>();

        public List<SceneData> Nodes => _nodes;
        public List<PassagePairData> Edges => _edges;
    }

}
