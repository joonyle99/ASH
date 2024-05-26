using UnityEngine;
using System.Collections.Generic;

namespace LevelGraph
{
    /// <summary>
    /// 입구 or 출구에 대한 데이터 클래스이며,
    /// 통로가 속한 씬에 대한 정보도 포함한다
    /// </summary>
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

    [CreateAssetMenu(fileName = "LevelGraphData", menuName = "Level Management/Level Graph Data")]
    public class LevelGraphData : ScriptableObject
    {
        [SerializeField] private List<SceneData> _nodes = new();                // node: scene에 대한 정보
        [SerializeField] private List<PassagePairData> _edges = new();          // edge: scene과 scene을 잇는 passage에 대한 정보

        public List<SceneData> Nodes => _nodes;
        public List<PassagePairData> Edges => _edges;

        /// <summary>
        /// From Passage Data를 통해서,
        /// To Passage Data를 구하는 함수
        /// </summary>
        /// <param name="fromPassageData"></param>
        /// <returns></returns>
        public PassageData GetExitPassageData(PassageData fromPassageData)
        {
            PassageData toPassageData = new PassageData();

            // edge에서 From Passage Data에 해당하는 Passage Data를 찾는다
            var match = _edges.FindIndex(data =>
                data.EntranceScene.SceneName == fromPassageData.SceneName &&
                data.EntrancePassage == fromPassageData.PassageName);

            // edge에서 From Passage Data를 찾지 못한 경우
            if (match == -1)
            {
                Debug.LogWarning($"Exit for {fromPassageData.SceneName} is {fromPassageData.PassageName} not set");

                string warningMassage = "Every Edges Data in Level Graph Data\n";

                foreach (var edge in _edges)
                {
                    warningMassage += $"\n {edge.EntranceScene.SceneName} / {edge.EntrancePassage} ========> {edge.ExitScene.SceneName} / {edge.ExitPassgage}";
                }

                Debug.LogWarningFormat(warningMassage);

                return fromPassageData;
            }

            // To Passage Data
            toPassageData.SceneName = _edges[match].ExitScene.SceneName;
            toPassageData.PassageName = _edges[match].ExitPassgage;

            return toPassageData;
        }
    }
}
