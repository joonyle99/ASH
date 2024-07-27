using System.Collections;
using UnityEngine;

public class LaserFlower : MonoBehaviour, IAttackListener
{
    [Header("Laser Flower")]
    [Space]

    [SerializeField] private DarkBeam _darkBeam;
    [SerializeField] private float _darkBeamHeadOffset = 0.1f;

    [Space]

    [SerializeField] private Transform _imageParent;
    [SerializeField] private Transform _headBone;
    [SerializeField] private GameObject _attackHitbox;

    [Space]

    [SerializeField] private int _maxHp = 3;
    [SerializeField] private float _rotation = 90;
    [SerializeField] private float _reviveDelay = 3;

    [Space]

    [SerializeField] private SoundList _soundList;

    private Animator _animator;
    private int _hp;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _hp = _maxHp;

        //AdjustAngles();
    }
    private void OnValidate()
    {
        AdjustAngles();
    }
    private void Update()
    {
        _darkBeam.transform.position = _headBone.position + new Vector3(Mathf.Cos(_rotation * Mathf.Deg2Rad), Mathf.Sin(_rotation * Mathf.Deg2Rad), 0) * _darkBeamHeadOffset;
    }

    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (_hp > 0 && attackInfo.Type == AttackType.Player_BasicAttack)
        {
            _soundList.PlaySFX("Hurt");

            if (--_hp <= 0)
                Close();

            return IAttackListener.AttackResult.Success;
        }
        return IAttackListener.AttackResult.Fail;
    }

    private void AdjustAngles()
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
    private void Open()
    {
        //_darkBeam.gameObject.SetActive(true);
        _hp = _maxHp;
        _animator.SetTrigger("Recover");
        _attackHitbox.SetActive(true);
        _soundList.PlaySFX("Revive");
    }
    private void Close()
    {
        if (!_darkBeam.gameObject.activeSelf)
            return;
        _darkBeam.gameObject.SetActive(false);
        _animator.SetTrigger("Die");
        _attackHitbox.SetActive(false);
        StartCoroutine(OpenCoroutine());
    }
    private IEnumerator OpenCoroutine()
    {
        yield return new WaitForSeconds(_reviveDelay);
        Open();
    }

    public void AnimEvent_ActivateDarkBeam()
    {
        _darkBeam.gameObject.SetActive(true);
    }
}
