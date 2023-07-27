using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack Setting")]

    [Space]

    [SerializeField]
    Transform _basicAttackHitbox;              // 공격 타격 박스

    [Range(0f, 5f)] [SerializeField]
    float _attackCountRefreshTime = 1.5f;      // 공격 초기화 시간

    PlayerBehaviour _player;

    int _basicAttackCount;              // 공격 카운트
    float _timeAfterLastBasicAttack;    // 마지막으로 공격한 시간

    /*
    [SerializeField] Slider slider;
    public int speed = 100;
    public float minPos;
    public float maxPos;
    public RectTransform pass;
    */

    public bool IsBasicAttacking { get; private set; }

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }
    private void OnEnable()
    {
        AnimEvent_FinishBaseAttackAnim();
    }

    public void CastBasicAttack()
    {
        print("CastBasicATtac: " + IsBasicAttacking);
        if (!IsBasicAttacking)
        {
            // Basic Attack Hitbox 활성화
            _basicAttackHitbox.gameObject.SetActive(true);

            // 사운드 재생
            _player.PlaySound_SE_Attack();

            // 마지막 공격 시간을 저장
            _timeAfterLastBasicAttack = Time.time;

            // 공격 횟수 증가
            _basicAttackCount++;

            // 공격 상태값 설정
            IsBasicAttacking = true;

            _player.Animator.SetTrigger("Attack");
            _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);

            // 공격 횟수를 애니메이션 종속으로,,
            if (_basicAttackCount >= 3)
                _basicAttackCount = 0;
        }
    }

    public void CastShootingAttack()
    {
        _player.ChangeState<ShootingState>();
    }

    private void Update()
    {
        // 1초 후 다시 처음으로
        if (Time.time >= _timeAfterLastBasicAttack + _attackCountRefreshTime)
        {
            _basicAttackCount = 0;
            _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);
        }
    }

    public void AnimEvent_FinishBaseAttackAnim()
    {
        // 공격 상태값 설정
        IsBasicAttacking = false;

        // Basic Attack Hitbox 비활성화
        _basicAttackHitbox.gameObject.SetActive(false);
    }

    /*
    public void PlayerBlenAnim(int num)
    {
        _player.Animator.SetFloat("Blend", num);
        _player.Animator.SetTrigger("Attack");
    }

    public void SetAttack()
    {
        slider.value = 0;
        minPos = pass.anchoredPosition.x;
        maxPos = pass.sizeDelta.x + minPos;
        StartCoroutine(ComboAtk());
    }

    IEnumerator MoveSlider()
    {
        while (!(Input.GetKeyDown(KeyCode.Space) || slider.value >= slider.maxValue))
        {
            slider.value += Time.deltaTime * speed;
            yield return null;
        }
    }

    IEnumerator ComboAtk()
    {
        yield return null;

        while (!(Input.GetKeyDown(KeyCode.Space) || slider.value == slider.maxValue))
        {
            slider.value += Time.deltaTime * speed;
            yield return null;
        }

        if (slider.value >= minPos && slider.value <= maxPos)
        {
            PlayerBlenAnim(0);
        }
    }
    */
}