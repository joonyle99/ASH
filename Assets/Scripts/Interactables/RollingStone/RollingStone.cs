using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RollingStone : InteractableObject
{
    public enum StoneType { RollingStone, StillStone}
    Rigidbody2D _rigidbody;

    [SerializeField] GameObject _playerInteractor;
    [SerializeField] float _maxRollSpeed;
    [SerializeField] float _maxInteractionDistance = 0.1f;
    [SerializeField] StoneType _type = StoneType.RollingStone;

    public StoneType Type { get { return _type; } }
    Collider2D _collider;

    AttackableEntity _attackableComponent;

    [SerializeField] SoundList _soundList;
    [SerializeField] float _pushSoundInterval;

    bool _isPushSoundPlaying = false;

    public bool IsBreakable { get { return _attackableComponent == null; } }
    bool _immovable
    {
        get { return _playerInteractor.activeSelf; }
        set
        {
            _playerInteractor.SetActive(value);
            if (value)
                gameObject.layer = LayerMask.NameToLayer("Default");
            else
                gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _attackableComponent = GetComponent<AttackableEntity>();
        _immovable = true;
    }
    protected override void OnInteract()
    {
        _immovable = false;

        //TODO : Joint 생성
        //SceneContext.Current.Player.AddJoint<DistanceJoint2D>(_rigidbody, 300);

    }
    IEnumerator PlayPushSoundCoroutine(string key, float interval)
    {
        _isPushSoundPlaying = true;
        _soundList.PlaySFX(key);
        yield return new WaitForSeconds(interval);
        _isPushSoundPlaying = false;
    }

    private void Update()
    {
        if (_rigidbody.velocity.sqrMagnitude > 0.3f && _rigidbody.GetContacts(new Collider2D[1]) > 0)
        {
            if (!_isPushSoundPlaying)
                StartCoroutine(PlayPushSoundCoroutine("Roll", _pushSoundInterval));
        }
    }
    public override void UpdateInteracting()
    {

        if (_rigidbody.velocity.sqrMagnitude > _maxRollSpeed * _maxRollSpeed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxRollSpeed;
        }
        //TODO : 플레이어와 떨어질 때 joint 끊기
        if (Physics2D.Distance(_collider, SceneContext.Current.Player.MainCollider).distance > _maxInteractionDistance
            ||  InputManager.Instance.InteractionKey.State == KeyState.KeyUp)
        {
            _immovable = true;

            //SceneContext.Current.Player.RemoveJoint();

             FinishInteraction();
        }
    }

#if UNITY_EDITOR
    public void ApplyShape()
    {
        _playerInteractor.GetComponent<PolygonCollider2D>().points
            = GetComponent<PolygonCollider2D>().points;

    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(RollingStone))]
public class CubeGenerateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RollingStone t = (RollingStone)target;
        if (GUILayout.Button("Apply Shape"))
        {
            t.ApplyShape();
        }
    }
}
#endif