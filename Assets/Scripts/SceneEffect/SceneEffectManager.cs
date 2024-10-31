using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEffectManager : HappyTools.SingletonBehaviourFixed<SceneEffectManager>, ISceneContextBuildListener
{
    /// <summary>
    /// <para>
    /// Idle:
    /// �ƹ��� ī�޶� ȿ���� ���� default ����.
    /// ī�޶� �÷��̾� �Ѿƴٴ�.
    /// </para>
    /// 
    /// <para>
    /// MajorEvent:
    /// Idle�� Cutscene ������ ���·�, ������Ʈ���� ���� �۵������� ī�޶� ȿ���� �Ϻ� ����� ����.
    /// ī�޶� focus�� �ٲ�ų� shake �Ǵ� ���� ȿ���� ����� �� ����.
    /// �ٸ� �켱������ �� ���� MajorEvent�� �ƽ����� override �� �� ����.
    /// ���� MajorEvent �߿��� ���� ���� Event�� ������ �����.
    /// �ƾ����� override�� ���, �ƾ��� ����� �� ���ƿ�.
    /// �ٸ� MajorEvent�� ���ÿ� ����� �� �ִ°� ���� ! (�� �����̴� ��ü�� ��� ī�޶� �ȿ� ��ƾ��� �� ��)
    /// ���� �켱������ (Ȥ�� �ƿ� ����) MajorEvent�� ��, ���߰Ű� override �ؾ��� ���� �ְ�, ���ľ��� ���� ����.
    /// ���� �켱������ �� ������� �����ؾ��� -> �⺻�� override, �����ϸ� ��ġ�� �����ϰ�����.
    /// ��ġ��� ? ���ÿ� Ʈ�� ���� ��. ���ۼ����� ��������� �޶� �ǵ��Ѵ�� �Ǿ����.
    /// </para>
    /// 
    /// <para>
    /// Cutscene:
    /// �ƾ� ������ ������ ������Ʈ�� �ٸ� �ֵ��� �۵����� ����, UI�� �����.
    /// ������ �ƾ��� ����ϸ� �ٸ� ���³� �ٸ� �ƽ����� override �� �� ���� (�ƾ� �� �ٸ� �ƾ��� ȣ����� ����)
    /// Cutscene�� �ڷ�ƾ, MajorEvent�� State Machine ó�� ����
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
            // ��� SceneEvent�� �����Ѵ� (������Ʈ)
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

        // �ƾ��� ���� ��� �ٷ� ���
        if (_cutSceneQueue.Count == 0)
        {
            PlayCutscene(cutscene);
        }
        // �ƾ��� �ִ� ��� ť�� �߰�
        else
        {
            _cutSceneQueue.Add(cutscene);
        }

        // �ƾ��� ����Ǵ� ���� �ٸ� �̺�Ʈ���� ��Ȱ��ȭ
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
            // �ƾ� ot �� �̺�Ʈ�� ���� ��� �⺻ ���·� ����
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
