using NaughtyAttributes.Test;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossLantern : Lantern
{
    [Header("Boss Lantern")]
    [SerializeField] private List<Explodable> _explodables;
    [SerializeField] private CutscenePlayer[] _cutscenePlayers;
    [SerializeField] private ChasingCamera _chasingCamera;

    public static bool IsPlayingLostLantern = false;

    protected override void Awake()
    {
        base.Awake();
        _cutscenePlayers = FindObjectsByType<CutscenePlayer>(FindObjectsSortMode.None);
        _chasingCamera = FindObjectOfType<ChasingCamera>();
    }

    protected override void FixedUpdate()
    {
        bool canLostLantern = !IsPlayingLostLantern && !IsLightOn && !IsTargetVisible();
        if (canLostLantern)
        {
            if(_chasingCamera != null && _chasingCamera.DeadLinePosY > transform.position.y)
            {
                Debug.Log($"Dead line : {_chasingCamera.DeadLinePosY}, {gameObject.name} lanternPosition : {transform.position.y}");
                LostLantern();
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void LostLantern()
    {
        IsPlayingLostLantern = true;
        StartCoroutine(LostLanternLogic());
    }

    private IEnumerator LostLanternLogic()
    {
        Debug.Log($"Start Lost lantern logic");
        CameraController cameraController = SceneContext.Current.CameraController;

        cameraController.StartFollow(transform);
        cameraController.UpdateScreenSize(5f);

        yield return new WaitUntil(() => cameraController.GetComponent<Camera>().velocity.magnitude < 0.1f);

        foreach(var explodeTarget  in _explodables)
        {
            explodeTarget.explode();
        }

        yield return new WaitForSeconds(3f);

        PlayCutscene("CutscenePlayer_Fire_Lantern_Lost");
    }

    //해당 함수를 CutscenePlayer_Fire_Lantern_Lost컷씬 제일 마지막에 넣어 줘야함, 그렇지 않으면 ChasingCamera의
    //EndChasing()에 의해 시점이 바뀜
    public void EndOfLostLantern()
    {
        IsPlayingLostLantern = false;
        //Debug.Log($"End Lost lantern logic");
    }



    #region CheckState
    private bool IsTargetVisible()
    {
        Camera camera = SceneContext.Current.CameraController.GetComponent<Camera>();

        var planes = GeometryUtility.CalculateFrustumPlanes(camera);
        var point = transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }

        return true;
    }
    #endregion

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
