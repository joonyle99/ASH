using System.Collections.Generic;

public class GameSceneManager : HappyTools.SingletonBehaviourFixed<GameSceneManager>
{
    // HashSet�� �ؽ����̺��� ����ϰ�, �ַ� ������ �˻翡 ����մϴ� (����� ���� ���θ� ������ Ȯ���� �� �ֽ��ϴ�)
    // -> �ߺ� ��Ҹ� ������� �ʴ´�
    // Dictionary�� �ؽ����̺��� ����ϰ�, Ű�� �̿��� '��'�� �˻��ϴ� �� ����մϴ�
    // -> �ߺ� ���� ����ϳ�, �ߺ� Ű�� ������� �ʴ´�

    private static HashSet<string> openingScene = new HashSet<string>
    {
        "TitleScene", "PrologueScene"
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
    // TEMP
    private static HashSet<string> testScenes = new HashSet<string>
    {
        // �߰��� �׽�Ʈ ���� '���� �̸����ε� Passage'�� �����ؾ� �մϴ�.
        // e.g) Passage Name: "MyScene 1"

        "MyScene 1", "�ؿ� ��", "���� ��"
    };

    public static bool IsOpeningScene(string sceneName)
    {
        return openingScene.Contains(sceneName);
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
    // TEMP
    public static bool IsTestScene(string sceneName)
    {
        return testScenes.Contains(sceneName);
    }
    public static bool IsDefinedScene(string sceneName)
    {
        return IsOpeningScene(sceneName) || IsExploration1(sceneName) || IsExploration2(sceneName) ||
               IsBossDungeon1(sceneName) || IsBossDungeon2(sceneName) || IsBossScene(sceneName) ||
               IsTestScene(sceneName);
    }
}
