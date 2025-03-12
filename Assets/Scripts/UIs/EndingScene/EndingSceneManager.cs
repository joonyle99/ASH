using HappyTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneManager : SingletonBehaviourFixed<EndingSceneManager>
{
    [SerializeField] private List<string> _surroundingSceneNames;
    [SerializeField] private float _sceneTransitionDuration = 3f;
    [SerializeField] private float _sceneSurroundingDuration = 3f;

    [Space]

    public CameraFollowTarget CameraFollowTarget;

    private CutscenePlayer[] _cutscenePlayers;

    private SceneTransitionPlayer _transitionPlayer;
    private string _endingSceneName;

    private void Start()
    {
        Initialzie();

        PlayCutscene("EndingCutscene_SurroundingScene");
    }

    private void Initialzie()
    {
        _cutscenePlayers = FindObjectsOfType<CutscenePlayer>();
        _endingSceneName = SceneManager.GetActiveScene().name;

        SceneContext.Current.DefaultBuild();
        _transitionPlayer = SceneContext.Current.SceneTransitionPlayer;
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(_transitionPlayer.FadeCoroutine(duration, SceneTransitionPlayer.FadeType.Lighten));
    }
    public void FadeOut(float duration)
    {
        StartCoroutine(_transitionPlayer.FadeCoroutine(duration, SceneTransitionPlayer.FadeType.Darken));
    }

    private void PlayCutscene(string cutsceneName)
    {
        for (int i = 0; i < _cutscenePlayers.Length; i++)
        {
            if (cutsceneName == _cutscenePlayers[i].name)
            {
                _cutscenePlayers[i].Play();
                return;
            }
        }
    }

    public void ChangeSceneForSurrounding()
    {
        StartCoroutine(ChangeSceneForSurroundingCoroutine());
    }
    private IEnumerator ChangeSceneForSurroundingCoroutine()
    {
        var halfDuration = _sceneTransitionDuration / 2;

        var darken = SceneTransitionPlayer.FadeType.Darken;
        var lighten = SceneTransitionPlayer.FadeType.Lighten;

        // 순차적으로 씬을 변경하며 , 씬을 둘러보는 연출
        for (int i = 0; i < _surroundingSceneNames.Count; i++)
        {
            // darken
            yield return _transitionPlayer.FadeCoroutine(halfDuration, darken);

            // change to surrounding scene
            SceneChangeManager.Instance.ChangeToNonPlayableScene(_surroundingSceneNames[i], () => CreateFollowTarget());

            Debug.Log("come on 1");

            yield return new WaitForSeconds(_sceneSurroundingDuration);

            Debug.Log("come on 2");

            // lighten
            yield return _transitionPlayer.FadeCoroutine(halfDuration, lighten);
        }

        // 엔딩씬으로 돌아와서, 엔딩 크레딧 연출
        {
            // darken
            yield return _transitionPlayer.FadeCoroutine(halfDuration, darken);

            // change to ending scene
            SceneChangeManager.Instance.ChangeToNonPlayableScene(_endingSceneName, () => EndOfSurroudingScene());

            // lighten
            yield return _transitionPlayer.FadeCoroutine(halfDuration, lighten);
        }
    }

    private void CreateFollowTarget(Vector3? newDir = null, float speed = 5f)
    {
        var followTarget = FindObjectOfType<CameraFollowTarget>();
        if (followTarget == null)
        {
            followTarget = Instantiate(CameraFollowTarget);
        }
        followTarget.SetData(newDir ?? Vector3.right, speed);
        followTarget.SetTrigger(true);

        var cameraController = SceneContext.Current.CameraController;

        cameraController.MainCamera.fieldOfView = 80f; // TODO.. 
        cameraController.CurrentCameraType = CameraController.CameraType.Surrounding;
        cameraController.StartFollow(followTarget.transform);
    }
    private void EndOfSurroudingScene()
    {
        var cameraController = SceneContext.Current.CameraController;

        cameraController.MainCamera.fieldOfView = 60f;
        cameraController.CurrentCameraType = CameraController.CameraType.Normal;

        //CreateFollowTarget(Vector3.up);

        //CameraFollowTarget cameraFollowTarget = FindObjectOfType<CameraFollowTarget>();
        //if (cameraFollowTarget != null)
        //{
        //    SceneContext.Current.CameraController.StartFollow(cameraFollowTarget.transform);
        //}
        //else
        //{
        //    CreateFollowTarget();
        //}

        _cutscenePlayers = FindObjectsOfType<CutscenePlayer>();

        PlayCutscene("EndingCutscene_Final");
    }
}
