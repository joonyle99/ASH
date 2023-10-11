using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
    public Transform respawnPoint;
    [Range(0f, 10f)] public float disapearTime;
    [Range(0f, 10f)] public float reviveDelay;
    private SpriteRenderer[] renderers;
    private float[] startAlphas;
    private float time;


    protected override void OnEnter()
    {
        time = 0f;
        Animator.SetBool("IsDead", true);
        // Player.PlaySound_SE_Die_01();
        this.GetComponent<Collider2D>().enabled = false;
        renderers = GetComponentsInChildren<SpriteRenderer>(false);
        startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            startAlphas[i] = renderers[i].color.a;
    }

    protected override void OnUpdate()
    {
        if (time < disapearTime)
        {
            time += Time.deltaTime;
            float normalizedTime = time / disapearTime;

            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = renderers[i].color;
                color.a = Mathf.Lerp(startAlphas[i], 0f, normalizedTime);
                renderers[i].color = color;
                Player.CapeRenderer.material.SetFloat("_Opacity", 1 - normalizedTime);
            }
        }
        else
        {
            Player.PlaySound_SE_Die_02();
            Animator.SetBool("IsDead", false);
            gameObject.SetActive(false);
            //TEMP
            SceneContext.Current.InstantRespawn();
        }
    }

    protected override void OnExit()
    {

    }
}

