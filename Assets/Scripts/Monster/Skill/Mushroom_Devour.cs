using UnityEngine;

public class Mushroom_Devour : Monster_BodySkill
{
    [SerializeField]
    private CutscenePlayerList _cutscenePlayerList;

    protected override void Awake()
    {
        base.Awake();

        if (_cutscenePlayerList)
        {
            if (_cutscenePlayerList.FindCutscene("Lighting Guide"))
            {
                Debug.Log($"{name} �ν��Ͻ����� LightCutscene�� ����մϴ�");

                monsterSkillCutScene -= LightCutscene;
                monsterSkillCutScene += LightCutscene;
            }
        }
    }

    private void LightCutscene()
    {
        Debug.Log("LightCutscene");

        // �ƾ��� ���
        _cutscenePlayerList.PlayCutscene("Lighting Guide");

        // �̺�Ʈ�� �� ���� ȣ���ϵ��� ����
        monsterSkillCutScene -= LightCutscene;
    }
}
