using System.Collections;
using UnityEngine;

public class Monster_AreaSkill : Monster_Skill
{
    [Header("Area Skill")]
    [Space]

    [SerializeField] float _lifeDuration;

    void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(_lifeDuration);
        Destroy(gameObject);
    }
}
