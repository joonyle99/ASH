using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear_Slash : Monster_SkillObject
{
    [SerializeField] float _duration;

    void Start()
    {
        StartCoroutine(DieCoroutine());
    }
    IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(_duration);
        Destroy(gameObject);
    }
}
