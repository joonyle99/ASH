using UnityEngine;
using UnityEngine.UI;

public class HealthPanelUI : MonoBehaviour
{
    [SerializeField] private Image[] _lifeIcons;
    [SerializeField] private Image[] _iconBackgrounds;

    private void Awake()
    {
        _iconBackgrounds = new Image[_lifeIcons.Length];
    }

    private void Start()
    {
        for (int i = 0; i < _lifeIcons.Length; i++)
        {
            var background = _lifeIcons[i].rectTransform.parent;
            _iconBackgrounds[i] = background.GetComponent<Image>();
        }
    }

    private void Update()
    {
        var player = SceneContext.Current.Player;

        if (player == null) return;

        UpdateLifeIcons(player.CurHp, player.MaxHp);
    }

    private void UpdateLifeIcons(int curHp, int maxHp)
    {
        // �� 10���� ���� �������� �����Ѵ�

        // ������ ���� �������� �̷��� ���� �� �ִ�
        // 1. �� ���ִ� ����
        // 2. ���ݸ� ���ִ� ����
        // 3. ����ִ� ����

        // �̰��� player curHp�� ��� ����ȭ�ؾ� �ұ�?

        // curHp��
        // 1, 2 -> 1��° ���� ������
        // 3, 4 -> 2��° ���� ������
        // 5, 6 -> ...
        // 7, 8
        // 9, 10
        // 11, 12
        // 13, 14
        // 15, 16
        // 17, 18
        // 19, 20 -> 10��° ���� ������ (19�̸� ���ݸ� ���ְ�, 20�̸� �� ���ִ�)

        // �÷��̾��� ���� ü�°� �ִ� ü���� Heath UI�� ����ȭ ��Ų��
        for (int i = 0; i < _lifeIcons.Length; i++)
        {
            // �� �����ܿ� �ش��ϴ� HP ���� ��� (1 ~ 20)
            int hpUnit = 2 * (i + 1); // 2, 4, 6, 8, 10, 12 ...

            // ���� ü�¿� ���� ����ȭ
            if (curHp >= hpUnit)
            {
                _lifeIcons[i].fillAmount = 1f;
            }
            else if (curHp >= hpUnit - 1)
            {
                _lifeIcons[i].fillAmount = 0.5f;
            }
            else
            {
                _lifeIcons[i].fillAmount = 0f;
            }

            // �ִ� ü�¿� ���� ����ȭ (���� �������� ����� ��Ʈ���ϸ�, ���ݸ� ���� ��찡 ����)
            if (maxHp >= hpUnit)
            {
                _iconBackgrounds[i].gameObject.SetActive(true);
            }
            else
            {
                _iconBackgrounds[i].gameObject.SetActive(false);
            }
        }
    }
}
