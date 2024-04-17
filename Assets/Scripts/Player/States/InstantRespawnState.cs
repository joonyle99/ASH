using System.Collections;
using UnityEngine;

public class InstantRespawnState : PlayerState
{
    [SerializeField] InputSetterScriptableObject _stayStillSetter;
    [SerializeField] float _spawnDuration;

    DisintegrateEffect_Old disintegrateEffectOld;
    public float DieDuration => disintegrateEffectOld.Duration;
    public float SpawnDuration => _spawnDuration;

    void Awake()
    {
        disintegrateEffectOld = GetComponent<DisintegrateEffect_Old>();
    }

    protected override bool OnEnter()
    {
        Player.Rigidbody.simulated = false;
        Player.enabled = false;
        Animator.speed = 0;
        InputManager.Instance.ChangeInputSetter(_stayStillSetter);
        disintegrateEffectOld.Play();
        Player.SoundList.PlaySFX("Disintegrate");

        return true;
    }
    protected override bool OnUpdate()
    {

        return true;
    }
    protected override bool OnExit()
    {
        InputManager.Instance.ChangeToDefaultSetter();

        return true;
    }

    IEnumerator SpawnCoroutine()
    {
        // change all player's spriteRenderer to original material
        Player.MaterialController.InitMaterial();

        // reset player condition
        Player.enabled = true;
        Animator.speed = 1;
        Player.Rigidbody.velocity = Vector2.zero;

        // 
        float eTime = 0f;
        while (eTime < _spawnDuration)
        {
            foreach (var spriteRenderer in Player.MaterialController.SpriteRenderers)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(0, 1, eTime / _spawnDuration);
                spriteRenderer.color = color;
            }
            eTime += Time.deltaTime;
            yield return null;
        }

        // TODO: ÀÌ ºÎºÐÀÌ ¹ºÁö ±î¸Ô¾ú´Ù
        /*
        Player.InitSpriteRendererAlpha();

        public void InitSpriteRendererAlpha()
        {
            foreach (var renderer in SpriteRenderers)
            {
                Color color = renderer.color;
                color.a = 1;
                renderer.color = color;
            }
        }
        */

        Player.Rigidbody.simulated = true;

        ChangeState<IdleState>();
    }
    public void Respawn()
    {
        StartCoroutine(SpawnCoroutine());
    }
}
