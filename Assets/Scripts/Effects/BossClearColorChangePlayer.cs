using System.Collections;
using UnityEngine;

public class BossClearColorChangePlayer : MonoBehaviour
{
    [SerializeField] Material _material;
    [SerializeField] float _duration = 2f;

    BossClearColorChange[] _changeTargets;

    public bool isEndEffect = false;

    void Awake()
    {
        // ���� �����ϴ� ��� BossClearColorChange ������Ʈ�� ã�Ƽ� �ʱ�ȭ�Ѵ�
        _changeTargets = FindObjectsOfType<BossClearColorChange>();
        foreach (var target in _changeTargets)
            target.Initialize(_material);
    }

    public void PlayEffect()
    {
        StartCoroutine(PlayEffectCoroutine());
    }
    IEnumerator PlayEffectCoroutine()
    {
        float eTime = 0f;
        while (eTime < _duration)
        {
            foreach (var target in _changeTargets)
                target.SetProgress(eTime / _duration);

            yield return null;

            eTime += Time.deltaTime;
        }

        isEndEffect = true;
    }
}
