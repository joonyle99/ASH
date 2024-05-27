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
                Debug.Log($"{name} 인스턴스에서 LightCutscene을 등록합니다");

                monsterSkillCutScene -= LightCutscene;
                monsterSkillCutScene += LightCutscene;
            }
        }
    }

    private void LightCutscene()
    {
        Debug.Log("LightCutscene");

        // 컷씬을 재생
        _cutscenePlayerList.PlayCutscene("Lighting Guide");

        // 이벤트를 한 번만 호출하도록 제거
        monsterSkillCutScene -= LightCutscene;
    }
}
