using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject, ISceneContextBuildListener
{
    [Header("Door")]

    [SerializeField] protected bool _isOpened = false;
    [SerializeField] protected InputSetterScriptableObject _enterInputSetter;     // 문으로 들어갈 때의 InputSetter

    protected DoorOpenAnimation _doorOpenAnimation;
    protected Animator _animator;
    protected BoxCollider2D _collider;
    protected SoundList _soundList;

    public virtual bool IsOpened
    {
        get => _isOpened;
        set
        {
            _isOpened = value;
        }
    }

    public virtual bool CanOpenDoor { get; set; } = false;

    protected virtual void Awake()
    {
        _doorOpenAnimation = GetComponent<DoorOpenAnimation>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _soundList = GetComponent<SoundList>();
    }

    public virtual void OnSceneContextBuilt() { }

    protected override void OnObjectInteractionEnter() { }

    protected virtual IEnumerator OpenDoorCoroutine(bool useCameraEffect)
    {
        if(useCameraEffect)
            SceneEffectManager.Instance.Camera.StartFollow(transform);

        yield return _doorOpenAnimation.OpenCoroutine();

        if(useCameraEffect)
            SceneEffectManager.Instance.Camera.StartFollow(SceneContext.Current.Player.transform);

        IsOpened = true;
    }

    protected virtual IEnumerator CloseDoorCoroutine(bool useCameraEffect)
    {
        if (useCameraEffect)
            SceneEffectManager.Instance.Camera.StartFollow(transform);

        yield return _doorOpenAnimation.CloseCoroutine();

        if (useCameraEffect)
            SceneEffectManager.Instance.Camera.StartFollow(SceneContext.Current.Player.transform);

        IsOpened = false;
    }

    public virtual void OpenDoor(bool useCameraEffect)
    {
        StartCoroutine(OpenDoorCoroutine(useCameraEffect));
    }

    public virtual void CloseDoor(bool useCameraEffect = false)
    {
        StartCoroutine(CloseDoorCoroutine(useCameraEffect));
    }

    public virtual void ToggleDoor(bool useCameraEffect = false)
    {
        if(IsOpened)
        {
            CloseDoor(useCameraEffect);
        }
        else
        {
            OpenDoor(useCameraEffect);
        }
    }


}
