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

    [SerializeField] protected float _threatVelocityThreshold = 3;
    [SerializeField] protected float _damage = 1;

    PolygonCollider2D _collider;

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
        _immovable = true;
    }
    protected override void OnInteract()
    {
        _immovable = false;
        //TODO : Joint ����
        SceneContext.Current.Player.AddJoint<HingeJoint2D>(_rigidbody, 300);
    }
    public override void UpdateInteracting()
    {
        //TODO : �÷��̾�� ������ �� joint ����
        if (
            InputManager.Instance.InteractionKey.State == KeyState.KeyUp)
        {
            _immovable = true; 
            SceneContext.Current.Player.RemoveJoint();
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