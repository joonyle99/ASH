public class Mushroom_Devour : Monster_BodySkill
{
    protected override void Awake()
    {
        base.Awake();

        monsterSkillEvent -= OnMushroomLightGuideEvent;
        monsterSkillEvent += OnMushroomLightGuideEvent;
    }

    // 머쉬룸 연출 이벤트
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