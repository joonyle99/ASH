using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EarthquakeEffect : MonoBehaviour
{
    [Header("Stalactite Setting")]
    //종유석을 큰 구획으로 몇 개 나눌지 지정
    [SerializeField] private int _sectorCount;
    [SerializeField] private Range _spawnDelayTime;
    [SerializeField] Transform _spawnPoint;

    [SerializeField] private List<Coroutine> _spawnSectorCoroutine = new List<Coroutine>();

    [Space(10), Header("Sound")]
    [SerializeField] private SoundList _soundList;

    [Space(10), Header("Camera Effect")]
    [SerializeField] private ConstantShakePreset _shakeEffect;

    [Space(10), Header("External Reference")]
    [SerializeField] private Stalactite _stalactitePrefab;

    private void Awake()
    {
        _soundList = GetComponent<SoundList>();
    }

    public void Play()
    {
        BossDungeonManager.Instance.OnDashObtainEventPlayed();

        if (_shakeEffect)
        {
            SceneEffectManager.Instance.Camera.StartConstantShake(_shakeEffect);
        }
        if (_soundList)
        {
            _soundList.PlaySFX("SE_Shaking");
        }
    }

    public void EndPlay()
    {
        if (_shakeEffect)
        {
            SceneEffectManager.Instance.Camera.StopConstantShake();
        }
        if (_soundList)
        {
            _soundList.StopRecentLoopPlayedSFX();
        }

        for(int i = 0; i < _sectorCount; i++)
        {
            StopCoroutine(_spawnSectorCoroutine[i]);
        }
    }

    public void PlayThudSound()
    {
        _soundList.PlaySFX("SE_Thud");
    }

    public void DropStalactite()
    {
        for (int i = 0; i < _sectorCount; i++)
        {
            _spawnSectorCoroutine.Add(StartCoroutine(DropStalactiteEachSectorLogic(i)));
        }
    }

    public IEnumerator DropStalactiteEachSectorLogic(int idx)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_spawnDelayTime.Start, _spawnDelayTime.End));

            float spawnStartPosX = _spawnPoint.position.x + (_spawnPoint.localScale.x / _sectorCount) * idx;
            float nextSpawnStartPosX = spawnStartPosX + (_spawnPoint.localScale.x / _sectorCount);
            spawnStartPosX = Random.Range(spawnStartPosX, nextSpawnStartPosX);
            Vector3 spawnPos = new Vector3(spawnStartPosX, _spawnPoint.position.y);

            SpawnStalactite(spawnPos);
        }
    }

    public void SpawnStalactite(Vector3 spawnPos)
    {
        if (_stalactitePrefab == null)
        {
            return;
        }

        GameObject stalactite = Instantiate(_stalactitePrefab.gameObject, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < _sectorCount; i++)
        {
            Gizmos.color = i % 2 == 0 ? Color.white : Color.black;

            float sectorSize = _spawnPoint.localScale.x / _sectorCount;
            float centerX = (sectorSize / 2) + i * sectorSize + _spawnPoint.position.x;
            Vector3 center = new Vector3(centerX, _spawnPoint.position.y);
            Vector3 size = new Vector3(sectorSize, _spawnPoint.localScale.y);
            Gizmos.DrawCube(center, size);
        }
    }
}
