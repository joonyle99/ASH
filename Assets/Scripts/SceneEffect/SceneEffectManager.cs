using HappyTools;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class SceneEffectManager : SingletonBehaviour<SceneEffectManager>, ISceneContextBuildListener
{
    List<SceneEffectEvent> _sceneEvents = new List<SceneEffectEvent>();

    List<Cutscene> _cutSceneQueue = new List<Cutscene>();

    RangeInt _activeSceneEventRange = new RangeInt(0,0);
    public CameraController Camera { get; private set; }

    SceneEventComparator _eventComparator = new SceneEventComparator();
    public void EnterIdleState()
    {
        Camera.ResetCameraSettings();
    }

    public void OnSceneContextBuilt()
    {
        Camera = UnityEngine.Camera.main.GetComponent<CameraController>();
        EnterIdleState();
    }

    public SceneEffectEvent PushSceneEvent(SceneEffectEvent sceneEvent)
    {
        int index = _sceneEvents.BinarySearch(sceneEvent, _eventComparator);
        if (index < 0)
            index = 0;
        _sceneEvents.Insert(index, sceneEvent);
        UpdateActiveSceneEventRange();
        if (_activeSceneEventRange.Contains(index))
            sceneEvent.OnEnter();
        return sceneEvent;
    }
    void Update()
    {
        foreach (var sceneEvent in _sceneEvents)
        {
            sceneEvent.OnUpdate();
        }
    }
    void UpdateActiveSceneEventRange()
    {
        if (_sceneEvents.Count == 0)
        {
            _activeSceneEventRange = new RangeInt(0, 0);
            return;
        }
        _activeSceneEventRange.Start = 0;
        _activeSceneEventRange.End = 1;

        SceneEffectEvent.EventPriority priority = _sceneEvents[0].Priority;
        for (int i=1; i< _sceneEvents.Count; i++)
        {
            if (_sceneEvents[i].Priority != priority)
                return;
            if (_sceneEvents[i].MergePolicyWithSamePriority == SceneEffectEvent.MergePolicy.PlayTogether)
                _activeSceneEventRange.End++;
            else
                return;
        }
    }
    public void RemoveSceneEvent(SceneEffectEvent sceneEvent)
    {
        int index = _sceneEvents.FindIndex(0, _sceneEvents.Count, x => x == sceneEvent);
        if (index >= 0)
        {
            if (_activeSceneEventRange.Contains(index))
                sceneEvent.OnExit();
            UpdateActiveSceneEventRange();
        }
    }

        class Cutscene { }
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
