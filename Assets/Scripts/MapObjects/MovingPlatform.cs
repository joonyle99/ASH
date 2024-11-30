using Unity.Burst.CompilerServices;
using UnityEngine;

/// <summary>
/// 퍼즐 기믹에 사용되는 움직이는 플랫폼
/// </summary>
public class MovingPlatform : ToggleableObject, ISceneContextBuildListener
{
    private enum Type { Vertical, Horizontal }

    [Header("───────── Moving Platform ─────────")]
    [Space]

    [SerializeField] private Type _type = Type.Vertical;
    [SerializeField] private float _speed = 1;
    [SerializeField] private LayerMask _playerLayer;

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
    [SerializeField]
    private float _travelDistance = 0f;
    //스위치의 활성화 여부(true이더라도 _hasPlayerAtPath가 true이면 플랫폼은 움직이지 않음)
    private bool _isMoving = false;
    //스위치 경로에 플레이어가 있는지 판별, 버닝비버 임시 코드
    private bool _hasPlayerAtPath = false;

    private PreserveState _statePreserver;
    private SceneEffectEvent _recentEvent;

    private void OnValidate()
    {
        GetComponent<SpriteRenderer>().sprite = _stoneSprites[(int)_type];
        GetComponent<BoxCollider2D>().size = _colliderSizes[(int)_type];
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _statePreserver = GetComponent<PreserveState>();
    }
    private void OnDestroy()
    {
        if (_statePreserver)
        {
            if (SceneChangeManager.Instance && SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
            {
                _statePreserver.SaveState("_travelDistance", _travelDistance);
            }

            SaveAndLoader.OnSaveStarted -= SaveTravelDistanceState;
        }
    }
    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                _travelDistance = _statePreserver.LoadState<float>("_travelDistanceSaved", 0f);
            }
            else
            {
                _travelDistance = _statePreserver.LoadState("_travelDistance", 0f);
            }

            SaveAndLoader.OnSaveStarted += SaveTravelDistanceState;
        }
    }

    private void Update()
    {
        if (_isMoving && !_hasPlayerAtPath)
        {
            if (!_moveAudio.isPlaying)
                _moveAudio.Play();
        }
        else
        {
            ///버닝비버 임시 코드
            if (_moveAudio.isPlaying)
                _moveAudio.Stop();
            ///
        }
    }
    private void FixedUpdate()
    {
        if (_isMoving && !_hasPlayerAtPath)
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

        ///버닝비버 임시코드
        var target = _path.GetPosition(_travelDistance);
        var direction = (target - transform.position).normalized;
        var rayLength = 0.5f;
        Vector2 origin = Vector2.zero, boxSize = Vector2.zero, dir = Vector2.zero;
        RaycastHit2D hit = new RaycastHit2D();
        if (_type == Type.Vertical)
        {
            origin = transform.position + direction * _colliderSizes[0].y / 2;
            boxSize = new Vector2(_colliderSizes[0].x, rayLength);
            dir = Vector2.up * direction;
            hit = Physics2D.BoxCast(origin, boxSize, 1f, dir, 0f, _playerLayer);
        }
        else if (_type == Type.Horizontal)
        {
            origin = transform.position + direction * _colliderSizes[0].x / 2;
            boxSize = new Vector2(rayLength, _colliderSizes[0].y);
            dir = Vector2.right * direction;
            hit = Physics2D.BoxCast(origin, boxSize, 1f, dir, 0f, _playerLayer);
        }

        if (!hit.collider)
        {
            _rigidbody.MovePosition(_path.GetPosition(_travelDistance));

            if (_hasPlayerAtPath)
                _hasPlayerAtPath = false;
        }
        else
        {
            Debug.Log("raycast hit target : " + hit.collider);

            if (!_hasPlayerAtPath)
                _hasPlayerAtPath = true;
        }
        ///
        //_rigidbody.MovePosition(_path.GetPosition(_travelDistance));
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

    private void SaveTravelDistanceState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState<float>("_travelDistanceSaved", _travelDistance);
        }
    }
}
