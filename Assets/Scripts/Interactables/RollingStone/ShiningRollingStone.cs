using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiningRollingStone : RollingStone
{
    [Space(10f), Header("Shining Stone Setting")]
    [SerializeField] private float _moveDistRequirePlayCutscene = 1f;
    [SerializeField] private CutscenePlayer _cutscenePlayer;

    private float startPosX = 0f;

    protected override void Awake()
    {
        base.Awake();
        startPosX = transform.position.x;
    }

    public override void UpdateInteracting()
    {
        if (!_cutscenePlayer.IsPlayed && Mathf.Abs(startPosX - transform.position.x) >= _moveDistRequirePlayCutscene)
        {
            _cutscenePlayer.Play();
        }
        base.UpdateInteracting();
    }
}
