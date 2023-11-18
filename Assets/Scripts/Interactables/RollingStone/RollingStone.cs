using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RollingStone : InteractableObject
{
    Rigidbody2D _rigidbody;

    [SerializeField] GameObject _playerInteractor;
    [SerializeField] float _maxInteractionDistance = 0.1f;

    PolygonCollider2D _collider;

    AttackableEntity _attackableComponent;

    // Own Interaction Type 세팅
    public InteractionType.Type ownInteractionType = InteractionType.Type.ROLL;

    public bool IsBreakable { get { return _attackableComponent == null; } }
    bool _immovable
    {
        get { return _playerInteractor.activeSelf; }
        set
        {
            _playerInteractor.SetActive(value);
            if (value)
                gameObject.layer = LayerMask.NameToLayer("ExceptPlayer");
            else
                gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<PolygonCollider2D>();
        _attackableComponent = GetComponent<AttackableEntity>();
        _immovable = true;
    }
    protected override void OnInteract()
    {
        _immovable = false;

        //TODO : Joint 생성
        //SceneContext.Current.Player.AddJoint<HingeJoint2D>(_rigidbody, 300);

        // Player에게 Own Interaction Type을 넘겨준다
        SceneContext.Current.Player.GetComponent<InteractionState>().SetInteractionType(ownInteractionType);
    }
    public override void UpdateInteracting()
    {
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