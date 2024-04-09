using UnityEngine;

public class Mushroom_Devour : Monster_Skill
{
    [SerializeField]
    private CutscenePlayerList _cutscenePlayerList;

    private void Awake()
    {
        if (_cutscenePlayerList)
        {
            monsterSkillEvent -= LightCutscene;
            monsterSkillEvent += LightCutscene;
        }
    }

    private void LightCutscene()
    {
        // �ƾ��� ���
        _cutscenePlayerList.PlayNextCutScene();

        // �̺�Ʈ�� �� ���� ȣ���ϵ��� ����
        monsterSkillEvent -= LightCutscene;
    }
}
