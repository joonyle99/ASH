using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear_Slash : Monster_Skill
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
