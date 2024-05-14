using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 플레이어의 체력 UI를 관리하는 클래스
/// </summary>
public class HealthPanelUI : MonoBehaviour
{
    [SerializeField] private Image[] _lifeIconsBacks;                   // 생명 아이콘 배경 이미지들
    [SerializeField] private Image[] _lifeIcons;                        // 생명 아이콘 이미지들

    private int[] _hpUnit;

    private PlayerBehaviour _player;

    private void Awake()
    {
        _hpUnit = new int[_lifeIconsBacks.Length];
        _lifeIcons = new Image[_lifeIconsBacks.Length];

        _player = FindFirstObjectByType<PlayerBehaviour>();

        if (_player)
        {
            _player.OnHealthChanged -= UpdateLifeIcons;
            _player.OnHealthChanged += UpdateLifeIcons;
        }
        else
        {
            Debug.LogError($"health panel ui에서 Player 정보를 찾지 못했습니다. (Player Name: {_player.gameObject.name})");
        }
    }

    private void Start()
    {
        for (int i = 0; i < _lifeIconsBacks.Length; i++)
        {
            // set hp unit
            _hpUnit[i] = 2 * (i + 1);

            // set icon backgrounds images
            _lifeIcons[i] = _lifeIconsBacks[i].rectTransform.GetChild(0).GetComponent<Image>();
        }
    }

    private void UpdateLifeIcons(int curHp, int maxHp)
    {
        // 플레이어의 체력에 변동이 있는 경우 호출되는 함수

        var lifeIconCount = _lifeIconsBacks.Length;

        for (int i = 0; i < lifeIconCount; i++)
        {
            // check maxHp icon show
            var isFrontShow = maxHp >= _hpUnit[i] - 1;
            _lifeIconsBacks[i].gameObject.SetActive(isFrontShow);

            if (!isFrontShow) continue;

            // how much curHp icon show
            if (curHp >= _hpUnit[i])
            {
                _lifeIcons[i].DOFillAmount(1f, 0.5f).SetEase(Ease.OutCirc);
            }
            else if (curHp >= _hpUnit[i] - 1)
            {
                _lifeIcons[i].DOFillAmount(0.5f, 0.5f).SetEase(Ease.OutCirc);
            }
            else
            {
                _lifeIcons[i].DOFillAmount(0f, 0.5f).SetEase(Ease.OutCirc);
            }
        }
    }
}
