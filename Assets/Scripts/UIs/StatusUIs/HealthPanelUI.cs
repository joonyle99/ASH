using UnityEngine;
using UnityEngine.UI;

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
            // Debug.Log($"health panel ui에 player가 등록되었습니다. {_player.gameObject.name}");

            _player.OnHealthChanged -= UpdateLifeIcons;
            _player.OnHealthChanged += UpdateLifeIcons;
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

    /// <summary>
    /// 매 프레임마다 플레이어의 생명 아이콘을 업데이트한다
    /// </summary>
    /// <param name="curHp"></param>
    /// <param name="maxHp"></param>
    private void UpdateLifeIcons(int curHp, int maxHp)
    {
        var lifeIconCount = _lifeIconsBacks.Length;

        for (int i = 0; i < lifeIconCount; i++)
        {
            // check maxHp icon show
            var isShow = maxHp >= _hpUnit[i] - 1;
            _lifeIconsBacks[i].gameObject.SetActive(isShow);

            if (!isShow) continue;

            // how much curHp icon show
            if (curHp >= _hpUnit[i]) _lifeIcons[i].fillAmount = 1f;
            else if (curHp >= _hpUnit[i] - 1) _lifeIcons[i].fillAmount = 0.5f;
            else _lifeIcons[i].fillAmount = 0f;
        }
    }
}
