using UnityEngine;

public class HealthPanelUI : MonoBehaviour
{
    [SerializeField] private Transform[] _lifeIcon;

    private void Update()
    {
        if (SceneContext.Current.Player == null) return;
        UpdateLifeIcons(SceneContext.Current.Player.CurHp);
    }

    private void UpdateLifeIcons(int curHp)
    {
        // CurHp와 LifeIcon을 동기화한다
        for (int lifeIndex = 0; lifeIndex < _lifeIcon.Length; lifeIndex++)
        {
            bool isLifeOn = lifeIndex < curHp;
            _lifeIcon[lifeIndex].gameObject.SetActive(isLifeOn);
        }
    }
}
