using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

/// <summary>
/// 플레이어의 체력 UI를 관리하는 클래스
/// </summary>
public class HealthPanelUI : MonoBehaviour
{
    [SerializeField] private Image[] _lifeIconsBacks;                   // 생명 아이콘 배경 이미지들
    [SerializeField] private Image[] _lifeIcons;                        // 생명 아이콘 이미지들

    private int[] _hpUnit;

    private PlayerBehaviour _player;

    [Header("Ring Effect")]
    [Space]

    [SerializeField] private GameObject _ringStart;
    [SerializeField] private GameObject _ringEnd;

    private void Awake()
    {
        _hpUnit = new int[_lifeIconsBacks.Length];
        _lifeIcons = new Image[_lifeIconsBacks.Length];

        for (int i = 0; i < _lifeIconsBacks.Length; i++)
        {
            // set hp unit
            _hpUnit[i] = 2 * (i + 1);       // 2, 4, 6, 8, 10, 12, 14, 16, 18, 20

            // set icon backgrounds images
            _lifeIcons[i] = _lifeIconsBacks[i].rectTransform.GetChild(0).GetComponent<Image>();
        }

        _player = FindFirstObjectByType<PlayerBehaviour>();

        if (_player)
        {
            _player.OnHealthChanged -= UpdateLifeIcons;
            _player.OnHealthChanged += UpdateLifeIcons;

            //_player.OnHealthChanged -= RecoverHealthRingEffect;
            //_player.OnHealthChanged += RecoverHealthRingEffect;
        }
        else
        {
            Debug.LogError($"health panel ui에서 Player 정보를 찾지 못했습니다. (Player Name: {_player.gameObject.name})");
        }
    }

    /// <summary> 플레이어의 체력에 변동이 있는 경우 호출되는 함수 </summary>
    private void UpdateLifeIcons(int curHp, int maxHp)
    {
        var lifeIconCount = _lifeIconsBacks.Length;

        for (int i = 0; i < lifeIconCount; i++)
        {
            // check if background icon show
            var isFrontShow = maxHp >= _hpUnit[i] - 1;

            // show or hide background icon
            _lifeIconsBacks[i].gameObject.SetActive(isFrontShow);

            // if background icon is not show, front is skip
            if (!isFrontShow) continue;

            // how much front icon show
            if (curHp >= _hpUnit[i])
            {
                // 체력이 꽉 찬다
                _lifeIcons[i].DOFillAmount(1f, 2f).SetEase(Ease.OutCirc);
                // Debug.Log($"{i + 1}번째 아이콘 : {_lifeIcons[i].fillAmount} -> {1f}");
            }
            else if (curHp >= _hpUnit[i] - 1)
            {
                // 체력이 절반만 찬다
                _lifeIcons[i].DOFillAmount(0.5f, 2f).SetEase(Ease.OutCirc);
                // Debug.Log($"{i + 1}번째 아이콘 : {_lifeIcons[i].fillAmount} -> {0.5f}");
            }
            else
            {
                // 체력이 차지 않는다
                _lifeIcons[i].DOFillAmount(0f, 2f).SetEase(Ease.OutCirc);
                // Debug.Log($"{i + 1}번째 아이콘 : {_lifeIcons[i].fillAmount} -> {0f}");
            }
        }
    }

    private void RecoverHealthRingEffect(int curHp, int maxHp)
    {
        StartCoroutine(RecoverHealthRingEffectCoroutine());
    }

    private IEnumerator RecoverHealthRingEffectCoroutine()
    {
        //_ringStart.Get(true);
        yield return new WaitForSeconds(2f);

        // * ring end effect *
        _ringEnd.SetActive(true);
        yield return new WaitForSeconds(2f);
        _ringEnd.SetActive(true);
    }
}
