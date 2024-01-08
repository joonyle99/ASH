using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public enum CameraPriority
{
    Lowest, LightDoorOpen, Cutscene, SceneChange, AlwaysOverride
}

public class CameraControlToken
{
    static CameraControlToken _currentOwningToken = null;
    static CameraTokenQueue _tokenQueue = new CameraTokenQueue();
    static bool IsNothingQueued { get { return _tokenQueue.IsEmpty && _currentOwningToken == null; } }
    CameraPriority _priority;
    public CameraController Camera { get; private set; } = null;
    public bool IsAvailable { get { return _currentOwningToken == this; } }
    public CameraControlToken(CameraPriority priority)
    {
        _priority = CameraPriority.Lowest;
        if (priority == CameraPriority.AlwaysOverride)
        {
            ChangeCurrentOwningToken(this);
        }
        else
        {
            if (_currentOwningToken == null)
                ChangeCurrentOwningToken(this);
            else
                _tokenQueue.Enqueue(this);
        }
    }

    public void Release(bool resetCameraStateIfNothingQueued = true)
    {
        if (resetCameraStateIfNothingQueued && _tokenQueue.IsEmpty)
        {
            Camera?.ResetCameraSettings();
        }
        _tokenQueue.Remove(this);
        ChangeCurrentOwningToken(_tokenQueue.Dequeue());
    }

    void ChangeCurrentOwningToken(CameraControlToken token)
    {
        if (_currentOwningToken != null)
            _currentOwningToken.Camera = null;
        _currentOwningToken = token;
        if (_currentOwningToken != null)
            _currentOwningToken.Camera = UnityEngine.Camera.main.GetComponent<CameraController>();
    }
    public static void ClearQueue()
    {
        _currentOwningToken = null;
        _tokenQueue.Clear();
    }

    class CameraTokenQueue
    {
        List<CameraControlToken> _queue = new List<CameraControlToken>();

        public void Enqueue(CameraControlToken token)
        {
            int idx = _queue.FindLastIndex(x => x._priority < token._priority);
            if (idx == -1)
                _queue.Add(token);
            else
                _queue.Insert(idx, token);
        }
        public CameraControlToken Dequeue()
        {
            if (_queue.Count == 0)
                return null;
            var token = _queue[0];
            _queue.RemoveAt(0);
            return token;
        }
        public void Remove(CameraControlToken token)
        {
            _queue.Remove(token);
        }
        public CameraControlToken Peek()
        {
            if (_queue.Count == 0)
                return null;
            return _queue[0];
        }
        public bool IsEmpty { get { return _queue.Count == 0; } }
        public void Clear()
        {
            _queue.Clear();
        }
    }
}