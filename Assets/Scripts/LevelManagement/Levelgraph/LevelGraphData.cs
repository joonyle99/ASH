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

        public PassageData GetExitPassageData(PassageData entrancePassageData)
        {
            PassageData exitPassageData = new PassageData();
            int match= _edges.FindIndex(data => data.EntranceScene.SceneName == entrancePassageData.SceneName && data.EntrancePassage == entrancePassageData.PassageName);
            if (match == -1)
            {
                Debug.LogWarning(string.Format("Exit for {0}/{1} not set", entrancePassageData.SceneName, entrancePassageData.PassageName));
                return entrancePassageData;
            }
            exitPassageData.SceneName = _edges[match].ExitScene.SceneName;
            exitPassageData.PassageName = _edges[match].ExitPassgage;

            return exitPassageData;
        }
    }
    public struct PassageData
    {
        public string SceneName;
        public string PassageName;
        public PassageData(string sceneName, string passageName)
        {
            SceneName = sceneName;
            PassageName = passageName;
        }
    }

}
