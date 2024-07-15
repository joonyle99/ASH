using UnityEngine;

public class CutsceneManager : HappyTools.SingletonBehaviourFixed<CutsceneManager>
{
    private CutscenePlayerList _cutscenePlayerList;

    [SerializeField] private bool _isMushroomCutscenePlayed = false;
    private static bool s_IsMushroomCutscenePlayed = false;
    public bool IsMushroomCutscenePlayed
    {
        get => s_IsMushroomCutscenePlayed;
        set
        {
            if(!s_IsMushroomCutscenePlayed)
            {
                s_IsMushroomCutscenePlayed = true;
                _isMushroomCutscenePlayed = true;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _cutscenePlayerList = GetComponent<CutscenePlayerList>();
    }

    public void PlayMushroomLightGuideCutscene()
    {
        if (IsMushroomCutscenePlayed) return;

        IsMushroomCutscenePlayed = true;

        _cutscenePlayerList.PlayCutscene("CutScenePlayer_Mushroom_LightGuide");
    }
}