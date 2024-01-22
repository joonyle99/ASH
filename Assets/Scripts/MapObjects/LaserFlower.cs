using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFlower : MonoBehaviour, IAttackListener
{
    [SerializeField] int _maxHp = 3;
    [SerializeField] float _reviveDelay = 3;
    [SerializeField] DarkBeam _darkBeam;
    int _hp;
    void Awake()
    {
        _hp = _maxHp;
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
    void Open()
    {
        _darkBeam.gameObject.SetActive(true);
        _hp = _maxHp;
    }
    void Close()
    {
        if (!_darkBeam.gameObject.activeSelf)
            return;
        _darkBeam.gameObject.SetActive(false);
        StartCoroutine(OpenCoroutine());
    }
    IEnumerator OpenCoroutine()
    {
        yield return new WaitForSeconds(_reviveDelay);
        Open();
    }
}
