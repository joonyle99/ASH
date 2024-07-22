using System.Collections.Generic;

public class GameSceneManager : HappyTools.SingletonBehaviourFixed<GameSceneManager>
{
    private static HashSet<string> Title = new HashSet<string>
    {
        "TitleScene"
    };
    private static HashSet<string> exploration1 = new HashSet<string>
    {
        "1-1", "1-2", "1-3"
    };
    private static HashSet<string> exploration2 = new HashSet<string>
    {
        "2-1", "2-2", "2-3"
    };
    private static HashSet<string> bossDungeon1 = new HashSet<string>
    {
        "Boss_1-1", "Boss_1-2", "Boss_1-3", "Boss_1-4"
    };
    private static HashSet<string> bossDungeon2 = new HashSet<string>
    {
        "Boss_2-1", "Boss_2-2", "Boss_2-3"
    };
    private static HashSet<string> bossScenes = new HashSet<string>
    {
        "Boss_Bear", "Boss_Fire"
    };

    public static bool IsTitle(string sceneName)
    {
        return Title.Contains(sceneName);
    }
    public static bool IsExploration1(string sceneName)
    {
        return exploration1.Contains(sceneName);
    }
    public static bool IsExploration2(string sceneName)
    {
        return exploration2.Contains(sceneName);
    }
    public static bool IsBossDungeon1(string sceneName)
    {
        return bossDungeon1.Contains(sceneName);
    }
    public static bool IsBossDungeon2(string sceneName)
    {
        return bossDungeon2.Contains(sceneName);
    }
    public static bool IsBossScene(string sceneName)
    {
        return bossScenes.Contains(sceneName);
    }
}
