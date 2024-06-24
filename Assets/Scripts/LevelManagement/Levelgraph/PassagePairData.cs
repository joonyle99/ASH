using System;
using UnityEngine;

namespace LevelGraph
{
    [Serializable]
    public struct PassagePairData
    {
        public SceneData EntranceScene;
        public SceneData ExitScene;

        [Tooltip("들어가는 곳")]
        public string EntrancePassage;
        [Tooltip("나오는 곳")]
        public string ExitPassgage;

        public PassagePairData(SceneData entranceScene, SceneData exitScene, string entrancePassage, string exitPassage)
        {
            EntrancePassage = entrancePassage;
            ExitPassgage = exitPassage;
            EntranceScene = entranceScene;
            ExitScene = exitScene;
        }
    }

}
