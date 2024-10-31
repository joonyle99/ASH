using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEffectManager : HappyTools.SingletonBehaviourFixed<SceneEffectManager>, ISceneContextBuildListener
{
    /// <summary>
    /// <para>
    /// Idle:
    /// 아무런 카메라 효과도 없는 default 상태.
    /// 카메라가 플레이어 쫓아다님.
    /// </para>
    /// 
    /// <para>
    /// MajorEvent:
    /// Idle과 Cutscene 사이의 상태로, 오브젝트들은 전부 작동하지만 카메라 효과가 일부 적용된 상태.
    /// 카메라 focus가 바뀌거나 shake 되는 등의 효과가 적용될 수 있음.
    /// 다른 우선순위가 더 높은 MajorEvent나 컷신으로 override 될 수 있음.
    /// 여러 MajorEvent 중에선 가장 높은 Event의 설정만 사용함.
    /// 컷씬으로 override된 경우, 컷씬이 종료된 후 돌아옴.
    /// 다른 MajorEvent와 동시에 실행될 수 있는게 있음 ! (두 움직이는 물체를 모두 카메라 안에 잡아야할 때 등)
    /// 같은 우선순위인 (혹은 아예 같은) MajorEvent일 때, 나중거가 override 해야할 수도 있고, 합쳐야할 수도 있음.
    /// 같은 우선순위일 때 어떻게할지 정의해야함 -> 기본은 override, 설정하면 합치기 가능하게하자.
    /// 합치기란 ? 동시에 트는 것일 뿐. 시작순서와 종료순서가 달라도 의도한대로 되어야함.
    /// </para>
    /// 
    /// <para>
    /// Cutscene:
    /// 컷씬 중으로 지정된 오브젝트외 다른 애들은 작동하지 않음, UI도 사라짐.
    /// 오로지 컷씬만 재생하며 다른 상태나 다른 컷신으로 override 될 수 없음 (컷씬 중 다른 컷씬이 호출되지 않음)
    /// Cutscene은 코루틴, MajorEvent는 State Machine 처럼 관리
    /// </para>
    /// </summary>
    private enum State { Idle, Cutscene, SceneEvent }

    private State _currentState = State.Idle;

    private List<Cutscene> _cutSceneQueue;              // Cutscene
    public List<Cutscene> CutsceneQueue => _cutSceneQueue;

    private List<SceneEffectEvent> _sceneEvents;        // SceneEvent
    private SceneEventComparator _eventComparator;

    private Action _onAdditionalBefore = null;
    public Action OnAdditionalBefore { get { return _onAdditionalBefore; } set { _onAdditionalBefore = value; } }
    private Action _onAdditionalAfter = null;
    public Action OnAdditionalAfter { get { return _onAdditionalAfter; } set { _onAdditionalAfter = value; } }

    private CameraController _currentCamera;
    public CameraController Camera
    {
        get
        {
            if (_currentCamera == null)
            {
                _currentCamera = UnityEngine.Camera.main.GetComponent<CameraController>();

                if (_currentCamera == null)
                {
                    UnityEngine.Debug.LogError($"CameraController is invalid");
                    return null;
                }
            }
            return _currentCamera;
        }
    }

    private Cutscene _recentCutscene = null;

    protected override void Awake()
    {
        base.Awake();

        _cutSceneQueue = new List<Cutscene>();
        _sceneEvents = new List<SceneEffectEvent>();
        _eventComparator = new SceneEventComparator();
    }
    private void Update()
    {
        if (_currentState == State.SceneEvent)
        {
            // 모든 SceneEvent를 실행한다 (업데이트)
            foreach (var sceneEvent in _sceneEvents)
            {
                if (sceneEvent.Enabled)
                    sceneEvent.OnUpdate();
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            string result = $"<color=yellow><Cutscene List></color>\n";

            if (_recentCutscene != null)
            {
                result += _recentCutscene.GetCutsceneName() + '\n';
            }

            foreach (var cutscene in _cutSceneQueue)
            {
                result += cutscene.GetCutsceneName() + '\n';
            }

            Debug.Log(result);
        }
#endif
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        _cutSceneQueue.Clear();
        _sceneEvents.Clear();

        _onAdditionalBefore = null;
        _onAdditionalAfter = null;
    }

    public void OnSceneContextBuilt()
    {
        if (_currentState == State.Idle)
            EnterIdleState();
    }
    private void EnterIdleState()
    {
        _recentCutscene = null;
        _currentState = State.Idle;
        Camera.ResetCameraSettings();
    }

    // cutscene
    public IEnumerator PushCutscene(Cutscene cutscene)
    {
        Debug.Log($"PushCutscene - {cutscene.Owner.name}");

        if (_recentCutscene != null)
        {
            yield return new WaitUntil(() => (_recentCutscene == null) || _recentCutscene.IsDone);
        }

        // 컷씬이 없는 경우 바로 재생
        if (_cutSceneQueue.Count == 0)
        {
            PlayCutscene(cutscene);
        }
        // 컷씬이 있는 경우 큐에 추가
        else
        {
            _cutSceneQueue.Add(cutscene);
        }

        // 컷씬이 재생되는 동안 다른 이벤트들은 비활성화
        DisableAllSceneEvents();
    }
    private void PlayCutscene(Cutscene cutscene)
    {
        _recentCutscene = cutscene;
        _currentState = State.Cutscene;
        cutscene.Play(CutsceneEndCallback, OnAdditionalBefore, OnAdditionalAfter);
    }
    private void CutsceneEndCallback()
    {
        if (_cutSceneQueue.Count > 0)
        {
            var nextCutScene = _cutSceneQueue[0];
            _cutSceneQueue.RemoveAt(0);
            PlayCutscene(nextCutScene);
        }
        else if (_sceneEvents.Count > 0)
        {
            RefreshSceneEventStates();
        }
        else
        {
            // 컷씬 ot 씬 이벤트가 없는 경우 기본 상태로 설정
            EnterIdleState();
        }
    }

    // scene event
    public SceneEffectEvent PushSceneEvent(SceneEffectEvent sceneEvent)
    {
        int index = Math.Max(0, _sceneEvents.BinarySearch(sceneEvent, _eventComparator));
        _sceneEvents.Insert(index, sceneEvent);

        if (_currentState != State.Cutscene)
            RefreshSceneEventStates();

        return sceneEvent;
    }
    public void RemoveSceneEvent(SceneEffectEvent sceneEvent)
    {
        int index = _sceneEvents.FindIndex(0, _sceneEvents.Count, x => x == sceneEvent);
        if (index < 0)
            return;
        _sceneEvents.RemoveAt(index);
        if (sceneEvent.Enabled)
            sceneEvent.Enabled = false;
        if (_currentState != State.Cutscene)
        {
            if (_sceneEvents.Count > 0)
                RefreshSceneEventStates();
            else
                EnterIdleState();
        }
    }
    private void DisableAllSceneEvents()
    {
        foreach (var sceneEvent in _sceneEvents)
            sceneEvent.Enabled = false;
    }
    private void RefreshSceneEventStates()
    {
        if (_sceneEvents.Count == 0)
            return;

        _currentState = State.SceneEvent;

        bool enable = true;
        for (int i = 0; i < _sceneEvents.Count; i++)
        {
            _sceneEvents[i].Enabled = enable;

            if (enable)
            {
                if (i + 1 < _sceneEvents.Count)
                {
                    if (_sceneEvents[i].Priority != _sceneEvents[i + 1].Priority
                       || _sceneEvents[i].MergePolicyWithSamePriority == SceneEffectEvent.MergePolicy.OverrideWithRecent)
                    {
                        enable = false;
                    }
                }
            }
        }
    }

    public static void StopPlayingCutscene()
    {
        if (Instance._recentCutscene != null &&
            Instance._recentCutscene.CutSceneCoreCoroutine != null)
        {
            Instance._recentCutscene.Owner.StopCoroutine(Instance._recentCutscene.CutSceneCoreCoroutine);
        }

        Instance._cutSceneQueue.Clear();
    }
}
