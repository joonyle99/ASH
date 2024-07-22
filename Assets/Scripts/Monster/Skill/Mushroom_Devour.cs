public class Mushroom_Devour : Monster_BodySkill
{
    protected override void Awake()
    {
        base.Awake();

        monsterSkillEvent -= OnMushroomLightGuideEvent;
        monsterSkillEvent += OnMushroomLightGuideEvent;
    }

    // �ӽ��� ���� �̺�Ʈ
    private void OnMushroomLightGuideEvent()
    {
        CutsceneManager.Instance.PlayMushroomLightGuideCutscene();

        monsterSkillEvent -= OnMushroomLightGuideEvent;
    }

    private void OnDestroy()
    {
        monsterSkillEvent -= OnMushroomLightGuideEvent;
    }
}