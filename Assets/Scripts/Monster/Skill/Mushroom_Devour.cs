using UnityEngine;

public class Mushroom_Devour : Monster_BodySkill
{
    [Header("Cutscene")]
    [SerializeField]
    private CutscenePlayer _mushroomLightGuideCutscenePlayer;

    protected override void Awake()
    {
        base.Awake();

        monsterSkillEvent -= OnMushroomLightGuideEvent;
        monsterSkillEvent += OnMushroomLightGuideEvent;
    }

    // �ӽ��� ���� �̺�Ʈ
    private void OnMushroomLightGuideEvent()
    {
        _mushroomLightGuideCutscenePlayer?.Play();

        monsterSkillEvent -= OnMushroomLightGuideEvent;
    }

    public override void OnDestroy()
    {
        monsterSkillEvent -= OnMushroomLightGuideEvent;
    }
}