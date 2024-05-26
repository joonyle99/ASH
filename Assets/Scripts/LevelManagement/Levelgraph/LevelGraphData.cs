using UnityEngine;
using System.Collections.Generic;

namespace LevelGraph
{
    /// <summary>
    /// �Ա� or �ⱸ�� ���� ������ Ŭ�����̸�,
    /// ��ΰ� ���� ���� ���� ������ �����Ѵ�
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
        [SerializeField] private List<SceneData> _nodes = new();                // node: scene�� ���� ����
        [SerializeField] private List<PassagePairData> _edges = new();          // edge: scene�� scene�� �մ� passage�� ���� ����

        public List<SceneData> Nodes => _nodes;
        public List<PassagePairData> Edges => _edges;

        /// <summary>
        /// From Passage Data�� ���ؼ�,
        /// To Passage Data�� ���ϴ� �Լ�
        /// </summary>
        /// <param name="fromPassageData"></param>
        /// <returns></returns>
        public PassageData GetExitPassageData(PassageData fromPassageData)
        {
            PassageData toPassageData = new PassageData();

            // edge���� From Passage Data�� �ش��ϴ� Passage Data�� ã�´�
            var match = _edges.FindIndex(data =>
                data.EntranceScene.SceneName == fromPassageData.SceneName &&
                data.EntrancePassage == fromPassageData.PassageName);

            // edge���� From Passage Data�� ã�� ���� ���
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
