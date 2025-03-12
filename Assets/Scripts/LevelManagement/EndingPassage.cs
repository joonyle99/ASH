using System.Collections;
using UnityEngine;

public class EndingPassage : TriggerZone
{
    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        StartCoroutine(ExitSceneCoroutine());
    }

    private IEnumerator ExitSceneCoroutine()
    {
        // * push cutscene
        Cutscene exitSceneCutscene = new Cutscene(this, ExitSceneCutsceneCoroutine(), false);
        StartCoroutine(SceneEffectManager.Instance.PushCutscene(exitSceneCutscene));

        // * wait cutscene
        yield return new WaitUntil(() => exitSceneCutscene.IsDone);

        // # change to next scene
        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.ChangeMap;
        SceneChangeManager.Instance.ChangeToNonPlayableScene("EndingScene_Peace");
    }
    private IEnumerator ExitSceneCutsceneCoroutine()
    {
        // 씬을 나가는 컷씬에서 플레이어의 입력을 받지 않도록 설정
        InputManager.Instance.ChangeToStayStillSetter();

        // 씬을 나가는 효과
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
    }
}
