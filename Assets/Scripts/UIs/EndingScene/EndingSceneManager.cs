using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField] private Transform _endingImagesParent;
     private List<Transform> _endingImageTransforms = new List<Transform>();

    [SerializeField] private float _sceneTransitionDuration = 3f;
    [SerializeField] private float _sceneSurroundingDuration = 5f;

    [Space]

    public CutscenePlayer[] cutscenePlayers;
    public GameObject CameraFollowTargetPrefab;
    private SceneTransitionPlayer _transitionPlayer;

    private void Start()
    {
        Initialize();

        PlayCutscene("EndingCutscene_SurroundingScene");
    }

    public void Initialize()
    {
        for (int i = 0; i < _endingImagesParent.childCount; i++)
        {
            _endingImageTransforms.Add(_endingImagesParent.GetChild(i));
        }

        SceneContext.Current.DefaultBuild();
        _transitionPlayer = SceneContext.Current.SceneTransitionPlayer;
    }

    //public void FadeIn(float duration)
    //{
    //    StartCoroutine(_transitionPlayer.FadeCoroutine(duration, SceneTransitionPlayer.FadeType.Lighten));
    //}
    //public void FadeOut(float duration)
    //{
    //    StartCoroutine(_transitionPlayer.FadeCoroutine(duration, SceneTransitionPlayer.FadeType.Darken));
    //}

    public void PlayCutscene(string cutsceneName)
    {
        Debug.Log($"Cutscene player count {cutscenePlayers.Length}");

        for (int i = 0; i < cutscenePlayers.Length; i++)
        {
            if (cutsceneName == cutscenePlayers[i].name)
            {
                Debug.Log("Cutscene start playing");
                cutscenePlayers[i].Play();
                break;
            }
        }
    }

    public void ChangeImagesForSurrounding()
    {
        StartCoroutine(ChangeImagesForSurroundingCoroutine());
    }

    private IEnumerator ChangeImagesForSurroundingCoroutine()
    {
        Debug.Log("Play chage image for surrounding coroutine");

        var halfDuration = _sceneTransitionDuration / 2;

        var darken = SceneTransitionPlayer.FadeType.Darken;
        var lighten = SceneTransitionPlayer.FadeType.Lighten;

        // 순차적으로 씬을 변경하며 , 씬을 둘러보는 연출
        for (int i = 0; i < _endingImageTransforms.Count; i++)
        {
            // TODO: 해당 씬에서는 처음부터 darken 상태로,, (한 프레임 불편함..)
            // darken
            yield return _transitionPlayer.FadeCoroutine(halfDuration, darken);

            if(i != 0)
                _endingImageTransforms[i - 1].gameObject.SetActive(false);
            _endingImageTransforms[i].gameObject.SetActive(true);

            // lighten
            yield return _transitionPlayer.FadeCoroutine(halfDuration, lighten);

            yield return new WaitForSeconds(_sceneSurroundingDuration);
        }

        // darken
        yield return _transitionPlayer.FadeCoroutine(halfDuration, darken);

        _endingImagesParent.gameObject.SetActive(false);

        // lighten
        yield return _transitionPlayer.FadeCoroutine(halfDuration, lighten);

        PlayCutscene("EndingCutscene_Final");
    }

    /* 기획상 폐기
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

            yield return new WaitForSeconds(_sceneSurroundingDuration);

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
            followTarget = Instantiate(CameraFollowTargetPrefab).GetComponent<CameraFollowTarget>();
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
    */
}
