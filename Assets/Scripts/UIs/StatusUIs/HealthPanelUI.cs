using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �÷��̾��� ü�� UI�� �����ϴ� Ŭ����
/// </summary>
public class HealthPanelUI : MonoBehaviour
{
    [SerializeField] private Image[] _lifeIconsBacks;                   // ���� ������ ��� �̹�����
    [SerializeField] private Image[] _lifeIcons;                        // ���� ������ �̹�����

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
            Debug.LogError($"health panel ui���� Player ������ ã�� ���߽��ϴ�. (Player Name: {_player.gameObject.name})");
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
        // �÷��̾��� ü�¿� ������ �ִ� ��� ȣ��Ǵ� �Լ�

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
