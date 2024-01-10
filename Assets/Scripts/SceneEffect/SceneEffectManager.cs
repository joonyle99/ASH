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
    //Idle : 그냥 평시, 아무런 카메라효과도 없는 default 상태. 카메라가 플레이어 쫓아다님. 이 상태는 사전정의될수있음

    //MajorEvent : 평시와 컷씬 사이로, 오브젝트들은 전부 작동하지만 카메라효과가 일부 적용된 상태. 카메라 focus가 바뀌거나
    //             shake 되는 등의 효과가 적용될 수 있음
    //             다른 우선순위가 더 높은 MajorEvent나 컷신으로 override 될 수 있음.
    //             -여러 MajorEvent 중에선 가장 높은 Event의 설정만 사용함
    //             -컷씬으로 override된 경우, 컷씬이 종료된 후 돌아옴
    //             -다른 MajorEvent와 동시에 실행될 수 있는게 있음! (두 움직이는 물체를 모두 카메라 안에 잡아야할 때 등)
    //             -같은 우선순위인 (혹은 아예같은) MajorEvent일 때, 나중거가 override 해야할 수도 있고, 합쳐야할 수도 있음   
    //              .
    //              같은 우선순위일 때 어떻게할지 정의해야함 -> 기본은 override, 설정하면 합치기 가능하게하자.
    //            - 합치기란 ? 동시에 트는 것일 뿐. 시작순서와 종료순서가 달라도 의도한대로 되어야함.

    //Cutscene : 컷씬 중으로 지정된 오브젝트외 다른 애들은 작동하지 않음, UI도 사라짐. 오로지 컷씬만 재생하며
    //           다른 상태나 다른 컷신으로 override 될 수 없음 (컷씬 중 다른 컷씬이 호출되지 않음)

    //Cutscene은 코루틴, MajorEvent는 statemachine처럼 관리

}
