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
        // 컷씬을 재생
        _cutscenePlayerList.PlayCutscene("Lighting Guide");

        // 이벤트를 한 번만 호출하도록 제거
        monsterSkillEvent -= LightCutscene;
    }
}
