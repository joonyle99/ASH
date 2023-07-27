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
        renderers = GetComponentsInChildren<SpriteRenderer>();
        startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            startAlphas[i] = renderers[i].color.a;
    }

    protected override void OnUpdate()
    {
        if (time < disapearTime)
        {
            time += Time.deltaTime;
            float normalizedTime = time / 2;

            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = renderers[i].color;
                color.a = Mathf.Lerp(startAlphas[i], 0f, normalizedTime);
                renderers[i].color = color;
            }
        }
        else
        {
            Player.PlaySound_SE_Die_02();
            Animator.SetBool("IsDead", false);
            gameObject.SetActive(false);
            SceneManager.Instance.ReactivatePlayerAfterDelay(respawnPoint.position, reviveDelay);
        }
    }

    protected override void OnExit()
    {

    }
}

