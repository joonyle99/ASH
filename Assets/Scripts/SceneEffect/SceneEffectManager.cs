using System.Collections.Generic;

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
    private enum State { Idle, SceneEvent, Cutscene }

    private State _currentState = State.Idle;
    private CameraController _currentCamera;
    private List<SceneEffectEvent> _sceneEvents;
    private List<Cutscene> _cutSceneQueue;
    private SceneEventComparator _eventComparator;
    public CameraController Camera
    {
        get
        {
            if (_currentCamera == null)
                _currentCamera = UnityEngine.Camera.main.GetComponent<CameraController>();
            return _currentCamera;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _sceneEvents = new List<SceneEffectEvent>();
        _cutSceneQueue = new List<Cutscene>();
        _eventComparator = new SceneEventComparator();
    }
    void Update()
    {
        if ( _currentState == State.SceneEvent)
        {
            foreach (var sceneEvent in _sceneEvents)
            {
                if (sceneEvent.Enabled)
                    sceneEvent.OnUpdate();
            }
        }
    }

    public void OnSceneContextBuilt()
    {
        if (_currentState == State.Idle )
            EnterIdleState();
    }
    public void EnterIdleState()
    {
        _currentState = State.Idle;
        Camera.ResetCameraSettings();
    }

    public void PushCutscene(Cutscene cutscene)
    {
        if (_cutSceneQueue.Count == 0)
            PlayCutscene(cutscene);
        else
            _cutSceneQueue.Add(cutscene);

        DisableAllSceneEvents();
    }
    public SceneEffectEvent PushSceneEvent(SceneEffectEvent sceneEvent)
    {
        int index = _sceneEvents.BinarySearch(sceneEvent, _eventComparator);
        if (index < 0)
            index = 0;

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

    void PlayCutscene(Cutscene cutscene)
    {
        _currentState = State.Cutscene;
        cutscene.Play(CutsceneEndCallback);
    }
    void CutsceneEndCallback()
    {
        if (_cutSceneQueue.Count > 0)
        {
            var nextCutScene = _cutSceneQueue[0];
            _cutSceneQueue.RemoveAt(0);
            PlayCutscene(nextCutScene);
        }
        else if (_sceneEvents.Count > 0)
            RefreshSceneEventStates();
        else
            EnterIdleState();
    }
    void DisableAllSceneEvents()
    {
        foreach (var sceneEvent in _sceneEvents)
            sceneEvent.Enabled = false;
    }
    void RefreshSceneEventStates()
    {
        if (_sceneEvents.Count == 0)
            return;
        _currentState = State.SceneEvent;

        bool enable = true;
        for(int i = 0; i< _sceneEvents.Count; i++)
        {
            _sceneEvents[i].Enabled = enable;

            if (enable && i+1 < _sceneEvents.Count &&
                (_sceneEvents[i].Priority != _sceneEvents[i + 1].Priority ||
                _sceneEvents[i].MergePolicyWithSamePriority == SceneEffectEvent.MergePolicy.OverrideWithRecent))
                enable = false;
        }
    }
}
