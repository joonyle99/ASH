using UnityEngine;
using UnityEngine.UI;

public class HealthPanelUI : MonoBehaviour
{
    [SerializeField] private Image[] _lifeIcons;

    private void Update()
    {
        if (SceneContext.Current.Player == null) return;
        UpdateLifeIcons(SceneContext.Current.Player.CurHp);
    }

    private void UpdateLifeIcons(int curHp)
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

        // �ʿ��� �����ܸ�ŭ ä���
        for (int i = 0; i < _lifeIcons.Length; i++)
        {
            // �� �����ܿ� �ش��ϴ� HP ���� ��� (1 ~ 20)
            int hpUnit = 2 * (i + 1); // 2, 4, 6, 8, 10, 12 ...

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
        }
    }
}
