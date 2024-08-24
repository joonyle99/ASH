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
        SceneEffectManager.Instance.PushCutscene(exitSceneCutscene);

        // * wait cutscene
        yield return new WaitUntil(() => exitSceneCutscene.IsDone);

        // # change to next scene
        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.ChangeMap;
        SceneChangeManager.Instance.ChangeToNonPlayableScene("EndingScene");
    }
    private IEnumerator ExitSceneCutsceneCoroutine()
    {
        // ���� ������ �ƾ����� �÷��̾��� �Է��� ���� �ʵ��� ����
        InputManager.Instance.ChangeToStayStillSetter();

        // ���� ������ ȿ��
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
    }
}
