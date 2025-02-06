using System;
using UnityEngine;
using UnityEngine.Events;

public enum PassageEnterBlockReason
{
    None,
    AlreadyObtainDashSkill,
}

public class ConditionalPassage : Passage
{
    [Space(10), Header("Conditional")]
    [SerializeField] protected PassageEnterBlockReason _blockReason = PassageEnterBlockReason.None;

    [SerializeField] private Func<bool> CanEnter;

    [SerializeField] private CutscenePlayer _blockedCutscene;

    private void Start()
    {
        ApplyEnterCondition(_blockReason);
    }

    public override void OnActivatorEnter(TriggerActivator activator)
    {
        if (_isPlayerExiting)
        {
            //Debug.Log("Player exit to passage");
            return;
        }

        if(CanEnter == null)
        {
            Debug.Log("Not allocated any delegate at 'Func<bool> CanEnter'");
            return;
        }

        if(!CanEnter())
        {
            _blockedCutscene?.Play();
        }
        else
        {
            base.OnActivatorEnter(activator);
        }

    }

    private void ApplyEnterCondition(PassageEnterBlockReason reason)
    {
        switch (reason)
        {
            case PassageEnterBlockReason.None:
                break;
            case PassageEnterBlockReason.AlreadyObtainDashSkill:
                OnEnterCondition_AlreadyObtainDashSkill();
                break;
        }
    }

    private void OnEnterCondition_AlreadyObtainDashSkill()
    {
        CanEnter += BossDungeonManager.Instance.DashObtainEventNotPlayed;
    }

    private void OnDestroy()
    {
        switch (_blockReason)
        {
            case PassageEnterBlockReason.None:
                break;
            case PassageEnterBlockReason.AlreadyObtainDashSkill:
                CanEnter -= BossDungeonManager.Instance.DashObtainEventNotPlayed;
                break;
        }
    }
}
