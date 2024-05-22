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
        // 씬에 존재하는 모든 BossClearColorChange 컴포넌트를 찾아서 초기화한다
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
        Debug.Log("색 변화 시작");

        float eTime = 0f;
        while (eTime < _duration)
        {
            foreach (var target in _changeTargets)
                target.SetProgress(eTime / _duration);

            yield return null;

            eTime += Time.deltaTime;
        }

        isEndEffect = true;

        Debug.Log("색 변화 종료");
    }
}
