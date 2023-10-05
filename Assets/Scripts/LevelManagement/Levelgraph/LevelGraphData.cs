using System;
using UnityEngine;
using System.Collections.Generic;


namespace LevelGraph
{
    [CreateAssetMenu(fileName = "LevelGraphData", menuName = "LevelManagement/Level Graph Data")]
    public class LevelGraphData : ScriptableObject
    {
        [SerializeField] List<SceneData> _nodes = new List<SceneData>();
        [SerializeField] List<PassagePair> _edges = new List<PassagePair>();

        public List<SceneData> Nodes => _nodes;
        public List<PassagePair> Edges => _edges;
    }

    [Serializable]
    public struct PassagePair
    {
        public SceneData EntranceScene;
        public SceneData ExitScene;
        public string EntrancePassage;
        public string ExitPassgage;

        public PassagePair(SceneData entranceScene, SceneData exitScene, string entrancePassage, string exitPassage)
        {
            EntrancePassage = entrancePassage;
            ExitPassgage = exitPassage;
            EntranceScene = entranceScene;
            ExitScene = exitScene;
        }
    }
}
