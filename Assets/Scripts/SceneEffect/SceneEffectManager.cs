using HappyTools;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class SceneEffectManager : MonoBehaviour, ISceneContextBuildListener
{
    enum State { Idle, SceneEvent, Cutscene }
    State _currentState = State.Idle;
    [SerializeField]List<SceneEffectEvent> _sceneEvents = new List<SceneEffectEvent>();

    List<Cutscene> _cutSceneQueue = new List<Cutscene>();

    public CameraController Camera { get; private set; }

    SceneEventComparator _eventComparator = new SceneEventComparator();
    public void OnSceneContextBuilt()
    {
        Camera = UnityEngine.Camera.main.GetComponent<CameraController>();
        if (_currentState == State.Idle )
            EnterIdleState();
    }
    public void EnterIdleState()
    {
        _currentState = State.Idle;
        Camera.ResetCameraSettings();
    }

    void UpdateCurrentSceneEffect()
    {
        _currentState = State.SceneEvent;
        _sceneEvents[0].Enabled = true;
        SceneEffectEvent.EventPriority priority = _sceneEvents[0].Priority;
        int sceneEventDisableStartIndex = 1;
        if (_sceneEvents[0].MergePolicyWithSamePriority == SceneEffectEvent.MergePolicy.PlayTogether)
        {
            for (int i = 1; i < _sceneEvents.Count; i++)
            {
                if (_sceneEvents[i].Priority == priority)
                {
                    _sceneEvents[i].Enabled = true;
                    sceneEventDisableStartIndex++;
                }
                else
                    break;
                if (_sceneEvents[i].MergePolicyWithSamePriority == SceneEffectEvent.MergePolicy.OverrideWithRecent)
                {
                    break;
                }
            }
        }
        for (int i = sceneEventDisableStartIndex; i < _sceneEvents.Count; i++)
            _sceneEvents[i].Enabled = false;
    }
    public SceneEffectEvent PushSceneEvent(SceneEffectEvent sceneEvent)
    {
        int index = _sceneEvents.BinarySearch(sceneEvent, _eventComparator);
        if (index < 0)
            index = 0;
        _sceneEvents.Insert(index, sceneEvent);

        if (_currentState != State.Cutscene)
            UpdateCurrentSceneEffect();
        return sceneEvent;
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
    public void RemoveSceneEvent(SceneEffectEvent sceneEvent)
    {
        int index = _sceneEvents.FindIndex(0, _sceneEvents.Count, x => x == sceneEvent);
        if (index >= 0)
        {
            _sceneEvents.RemoveAt(index);
            if (sceneEvent.Enabled)
                sceneEvent.Enabled = false;
            if (_currentState != State.Cutscene)
            {
                if (_sceneEvents.Count > 0)
                    UpdateCurrentSceneEffect();
                else
                    EnterIdleState();
            }
        }
    }

    public void PushCutscene(Cutscene cutscene)
    {
        _cutSceneQueue.Add(cutscene);
        foreach (var sceneEvent in _sceneEvents)
            sceneEvent.Enabled = false;
        _currentState = State.Cutscene;

        if (_cutSceneQueue.Count == 1)
        {
            StartFirstCutscene();
        }
    }
    void StartFirstCutscene()
    {
        var cutscene = _cutSceneQueue[0];
        cutscene.Play(this, () =>
        {
            _cutSceneQueue.Remove(cutscene);
            if (_cutSceneQueue.Count > 0)
                StartFirstCutscene();
            else if (_sceneEvents.Count > 0)
                UpdateCurrentSceneEffect();
            else
                EnterIdleState();
        });
    }

    //Idle : �׳� ���, �ƹ��� ī�޶�ȿ���� ���� default ����. ī�޶� �÷��̾� �Ѿƴٴ�. �� ���´� �������ǵɼ�����

    //MajorEvent : ��ÿ� �ƾ� ���̷�, ������Ʈ���� ���� �۵������� ī�޶�ȿ���� �Ϻ� ����� ����. ī�޶� focus�� �ٲ�ų�
    //             shake �Ǵ� ���� ȿ���� ����� �� ����
    //             �ٸ� �켱������ �� ���� MajorEvent�� �ƽ����� override �� �� ����.
    //             -���� MajorEvent �߿��� ���� ���� Event�� ������ �����
    //             -�ƾ����� override�� ���, �ƾ��� ����� �� ���ƿ�
    //             -�ٸ� MajorEvent�� ���ÿ� ����� �� �ִ°� ����! (�� �����̴� ��ü�� ��� ī�޶� �ȿ� ��ƾ��� �� ��)
    //             -���� �켱������ (Ȥ�� �ƿ�����) MajorEvent�� ��, ���߰Ű� override �ؾ��� ���� �ְ�, ���ľ��� ���� ����   
    //              .
    //              ���� �켱������ �� ������� �����ؾ��� -> �⺻�� override, �����ϸ� ��ġ�� �����ϰ�����.
    //            - ��ġ��� ? ���ÿ� Ʈ�� ���� ��. ���ۼ����� ��������� �޶� �ǵ��Ѵ�� �Ǿ����.

    //Cutscene : �ƾ� ������ ������ ������Ʈ�� �ٸ� �ֵ��� �۵����� ����, UI�� �����. ������ �ƾ��� ����ϸ�
    //           �ٸ� ���³� �ٸ� �ƽ����� override �� �� ���� (�ƾ� �� �ٸ� �ƾ��� ȣ����� ����)

    //Cutscene�� �ڷ�ƾ, MajorEvent�� statemachineó�� ����

}
