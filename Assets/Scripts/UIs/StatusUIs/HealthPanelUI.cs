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
        // 총 10개의 생명 아이콘이 존재한다

        // 각각의 생명 아이콘은 이렇게 나눌 수 있다
        // 1. 꽉 차있는 상태
        // 2. 절반만 차있는 상태
        // 3. 비어있는 상태

        // 이것을 player curHp와 어떻게 동기화해야 할까?

        // curHp가
        // 1, 2 -> 1번째 생명 아이콘
        // 3, 4 -> 2번째 생명 아이콘
        // 5, 6 -> ...
        // 7, 8
        // 9, 10
        // 11, 12
        // 13, 14
        // 15, 16
        // 17, 18
        // 19, 20 -> 10번째 생명 아이콘 (19이면 절반만 차있고, 20이면 꽉 차있다)

        // 필요한 아이콘만큼 채우기
        for (int i = 0; i < _lifeIcons.Length; i++)
        {
            // 각 아이콘에 해당하는 HP 범위 계산 (1 ~ 20)
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
