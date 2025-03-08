using HappyTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneManager : SingletonBehaviourFixed<EndingSceneManager>
{
    private CutscenePlayer[] _cutscenePlayers;

    public GameObject CameraFollowTargetPrefab;

    private SceneTransitionPlayer _sceneTransitionPlayer;
    private string _endingSceneName;

    [SerializeField] private List<string> _sceneName;
    [SerializeField] private float _sceneLookingTime = 3f;
    [SerializeField] private float _sceneTransitionDelayTime = 3f;

    private void Start()
    {
        Init();

        //PlayCutscene("EndingCutscene_LookScene");
    }

    private void Init()
    {
        _cutscenePlayers = FindObjectsOfType<CutscenePlayer>();
        _endingSceneName = SceneManager.GetActiveScene().name;
        SceneContext.Current.DefaultBuild();

        _sceneTransitionPlayer = SceneContext.Current.SceneTransitionPlayer;
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(_sceneTransitionPlayer.FadeCoroutine(duration, SceneTransitionPlayer.FadeType.Lighten));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(_sceneTransitionPlayer.FadeCoroutine(duration, SceneTransitionPlayer.FadeType.Darken));
    }

    public void ChangeSceneForLookingAll()
    {
        StartCoroutine(ChangeSceneForLookingLogic());
    }

    private IEnumerator ChangeSceneForLookingLogic()
    {
        for(int i = 0; i < _sceneName.Count; i++)
        {
            yield return StartCoroutine(_sceneTransitionPlayer.FadeCoroutine(_sceneTransitionDelayTime / 2, SceneTransitionPlayer.FadeType.Darken));

            SceneChangeManager.Instance.ChangeToNonPlayableScene(_sceneName[i], () => CreateChaseTarget(Vector3.right));

            yield return new WaitForSeconds(_sceneLookingTime);

            yield return StartCoroutine(_sceneTransitionPlayer.FadeCoroutine(_sceneTransitionDelayTime / 2, SceneTransitionPlayer.FadeType.Lighten));
        }

        yield return StartCoroutine(_sceneTransitionPlayer.FadeCoroutine(_sceneTransitionDelayTime / 2, SceneTransitionPlayer.FadeType.Darken));
        
        SceneChangeManager.Instance.ChangeToNonPlayableScene(_endingSceneName, () => EndOfLookingScene());
        
        yield return StartCoroutine(_sceneTransitionPlayer.FadeCoroutine(_sceneTransitionDelayTime / 2, SceneTransitionPlayer.FadeType.Lighten));
    }

    private void CreateChaseTarget(Vector3 newDir, float speed = 5f)
    {
        CameraFollowTarget followTarget = FindObjectOfType<CameraFollowTarget>();
        if (followTarget == null)
        {
            followTarget = Instantiate(CameraFollowTargetPrefab).GetComponent<CameraFollowTarget>();
        }

        followTarget.SetData(newDir, speed);
        SceneContext.Current.CameraController.GetComponent<Camera>().fieldOfView = 80f;
        SceneContext.Current.CameraController.CurrentCameraType = CameraController.CameraType.Looking;
        SceneContext.Current.CameraController.StartFollow(followTarget.transform);
    }

    private void EndOfLookingScene()
    {
        SceneContext.Current.CameraController.CurrentCameraType = CameraController.CameraType.Normal;
        SceneContext.Current.CameraController.GetComponent<Camera>().fieldOfView = 60f;
        CameraFollowTarget cameraFollowTarget = FindObjectOfType<CameraFollowTarget>();
        if(cameraFollowTarget != null)
        {
            SceneContext.Current.CameraController.StartFollow(cameraFollowTarget.transform);
        }
        else
        {
            CreateChaseTarget(Vector3.right);
        }

        _cutscenePlayers = FindObjectsOfType<CutscenePlayer>();

        PlayCutscene("EndingCutscene_Final");
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
}
