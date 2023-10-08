using System;

namespace LevelGraph
{
    [Serializable]
    public struct PassagePairData
    {
        public SceneData EntranceScene;
        public SceneData ExitScene;
        public string EntrancePassage;
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
