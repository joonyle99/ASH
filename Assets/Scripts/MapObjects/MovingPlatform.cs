using UnityEngine;

/// <summary>
/// ぷ闌 晦嗶縑 餌辨腎朝 遺霜檜朝 Ы概イ
/// </summary>
public class MovingPlatform : ToggleableObject
{
    private enum Type { Vertical, Horizontal }

    [Header("式式式式式式式式式 Moving Platform 式式式式式式式式式")]
    [Space]

    [SerializeField] private Type _type = Type.Vertical;
    [SerializeField] private float _speed = 1;

    [Space]

    [SerializeField] private WaypointPath _path;
    [SerializeField] private LightIcon _icon;

    [Space]

    [SerializeField] private Sprite[] _stoneSprites;
    [SerializeField] private Vector2[] _colliderSizes;

    [Space]

    [SerializeField] private AudioSource _moveAudio;
    [SerializeField] private SoundClipData _workSound;

    private Rigidbody2D _rigidbody;
    private float _travelDistance = 0f;
    private bool _isMoving = false;

    private PreserveState _statePreserver;
    private SceneEffectEvent _recentEvent;

    private void OnValidate()
    {
        GetComponent<SpriteRenderer>().sprite = _stoneSprites[(int)_type];
        GetComponent<BoxCollider2D>().size = _colliderSizes[(int)_type];
    }
    private void OnDestroy()
    {
        if (_statePreserver)
            _statePreserver.SaveState("_travelDistance", _travelDistance);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _statePreserver = GetComponent<PreserveState>();

        if (_statePreserver)
            _travelDistance = _statePreserver.LoadState("_travelDistance", 0f);
    }
    private void Update()
    {
        if (_isMoving)
        {
            if (!_moveAudio.isPlaying)
                _moveAudio.Play();
        }
    }
    private void FixedUpdate()
    {
        if (_isMoving)
        {
            if (IsOn)
            {
                _travelDistance += _speed * Time.fixedDeltaTime;
                if (_travelDistance > _path.TotalDistance)
                {
                    _travelDistance = _path.TotalDistance;
                    OnStop();
                }
            }
            else
            {
                _travelDistance -= _speed * Time.fixedDeltaTime;
                if (_travelDistance < 0)
                {
                    _travelDistance = 0;
                    OnStop();
                }
            }
        }
        _rigidbody.MovePosition(_path.GetPosition(_travelDistance));
    }

    protected override void OnTurnedOff()
    {
        if (!_isMoving)
        {
            _recentEvent = SceneEffectManager.Instance.PushSceneEvent(
                (SceneEffectEvent)new FollowObjects(SceneEffectEvent.EventPriority.MovingObjects,
                                                        SceneEffectEvent.MergePolicy.PlayTogether, transform));
        }
        _isMoving = true;
    }
    protected override void OnTurnedOn()
    {
        if (!_isMoving)
        {
            _recentEvent = SceneEffectManager.Instance.PushSceneEvent(
                (SceneEffectEvent)new FollowObjects(SceneEffectEvent.EventPriority.MovingObjects,
                                                        SceneEffectEvent.MergePolicy.PlayTogether, transform));
        }
        _isMoving = true;

        //SoundManager.Instance.PlaySFX(_workSound);
    }

    private void OnStop()
    {
        _isMoving = false;
        SceneEffectManager.Instance.RemoveSceneEvent(_recentEvent);
        _moveAudio.Stop();
    }

}
