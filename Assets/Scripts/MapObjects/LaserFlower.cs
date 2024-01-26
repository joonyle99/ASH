using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFlower : MonoBehaviour, IAttackListener
{
    [SerializeField] int _maxHp = 3;
    [SerializeField] float _reviveDelay = 3;
    [SerializeField] DarkBeam _darkBeam;
    [SerializeField] float _darkBeamHeadOffset = 0.1f;
    [SerializeField] float _rotation = 90;
    [SerializeField] Transform _imageParent;
    [SerializeField] Transform _headBone;
    [SerializeField] GameObject _attackHitbox;

    Animator _animator;
    int _hp;
    void Awake()
    {
        _hp = _maxHp;
        _animator = GetComponent<Animator>();


        //AdjustAngles();
    }
    void OnValidate()
    {
        AdjustAngles();
    }
    void Update()
    {
        _darkBeam.transform.position = _headBone.position + new Vector3(Mathf.Cos(_rotation * Mathf.Deg2Rad), Mathf.Sin(_rotation * Mathf.Deg2Rad), 0) * _darkBeamHeadOffset;

    }
    void AdjustAngles()
    {
        if (Mathf.Abs(Mathf.DeltaAngle(_rotation, 0)) < 90)
        {
            _imageParent.localScale = new Vector3(-Mathf.Abs(_imageParent.localScale.x), _imageParent.localScale.y, _imageParent.localScale.z);
            float flippedRotation = Mathf.DeltaAngle(_rotation, 180);
            _headBone.localRotation = Quaternion.Euler(0, 0, flippedRotation + 180 + 16);

            _darkBeam.transform.rotation = Quaternion.Euler(0, 0, _rotation);
            _darkBeam.transform.position = _headBone.position + new Vector3(Mathf.Cos(_rotation * Mathf.Deg2Rad), Mathf.Sin(_rotation * Mathf.Deg2Rad), 0) * _darkBeamHeadOffset;

        }
        else
        {
            _imageParent.localScale = new Vector3(Mathf.Abs(_imageParent.localScale.x), _imageParent.localScale.y, _imageParent.localScale.z);

            _headBone.localRotation = Quaternion.Euler(0, 0, _rotation + 180 + 16);

            _darkBeam.transform.rotation = Quaternion.Euler(0, 0, _rotation);
           
        }
    }
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (_hp > 0 && attackInfo.Type == AttackType.Player_BasicAttack)
        {
            if(--_hp <= 0)
                Close();
            return IAttackListener.AttackResult.Success;
        }
        return IAttackListener.AttackResult.Fail;
    }
    public void AnimEvent_ActivateDarkBeam()
    {
        _darkBeam.gameObject.SetActive(true);

    }
    void Open()
    {
        //_darkBeam.gameObject.SetActive(true);
        _hp = _maxHp;
        _animator.SetTrigger("Recover");
        _attackHitbox.SetActive(true);
    }
    void Close()
    {
        if (!_darkBeam.gameObject.activeSelf)
            return;
        _darkBeam.gameObject.SetActive(false);
        _animator.SetTrigger("Die");
        _attackHitbox.SetActive(false);
        StartCoroutine(OpenCoroutine());
    }
    IEnumerator OpenCoroutine()
    {
        yield return new WaitForSeconds(_reviveDelay);
        Open();
        
    }
}
