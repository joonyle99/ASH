using UnityEngine;
using UnityEngine.UI;

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
            // Debug.Log($"health panel ui�� player�� ��ϵǾ����ϴ�. {_player.gameObject.name}");

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
    /// �� �����Ӹ��� �÷��̾��� ���� �������� ������Ʈ�Ѵ�
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
