using System.Collections.Generic;

public class GameSceneManager : HappyTools.SingletonBehaviourFixed<GameSceneManager>
{
    // HashSet은 해시테이블을 사용하고, 주로 고유성 검사에 사용합니다 (요소의 존재 여부를 빠르게 확인할 수 있습니다)
    // -> 중복 요소를 허용하지 않는다
    // Dictionary는 해시테이블을 사용하고, 키를 이용해 '값'을 검색하는 데 사용합니다
    // -> 중복 값은 허용하나, 중복 키는 허용하지 않는다

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
    public static bool IsDefinedScene(string sceneName)
    {
        return IsTitle(sceneName) || IsExploration1(sceneName) || IsExploration2(sceneName) ||
               IsBossDungeon1(sceneName) || IsBossDungeon2(sceneName) || IsBossScene(sceneName);
    }
}
