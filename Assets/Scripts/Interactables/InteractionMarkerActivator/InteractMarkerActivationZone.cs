using UnityEngine;

/// <summary>
/// 플레이어와의 상호작용을 위한 트리거존
/// 해당 트리거존은 '상호작용이 가능한 오브젝트'의 자식으로 존재한다
/// </summary>
public class InteractMarkerActivationZone : TriggerZone
{
    [SerializeField] private InteractableObject _interactionObject;

    private void Awake()
    {
        if (_interactionObject == null)
            _interactionObject = GetComponentInParent<InteractableObject>();

        if (_interactionObject == null)
            _interactionObject = GetComponent<InteractableObject>();

        if (_interactionObject == null)
            _interactionObject = GetComponentInChildren<InteractableObject>();
    }

    /// <summary>
    /// 해당 트리거존에 플레이어가 들어왔을 때 호출되는 함수
    /// </summary>
    /// <param name="player"></param>
    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        // 플레이어와 인접한 상호작용 가능한 오브젝트 리스트에 추가한다
        player.PlayerInteractionController.AddInteractionTarget(_interactionObject);
    }
    /// <summary>
    /// 해당 트리거존에서 플레이어가 나갔을 때 호출되는 함수
    /// </summary>
    /// <param name="player"></param>
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        // 플레이어와 인접한 상호작용 가능한 오브젝트 리스트에서 제거한다
        player.PlayerInteractionController.RemoveInteractionTarget(_interactionObject);
    }
}