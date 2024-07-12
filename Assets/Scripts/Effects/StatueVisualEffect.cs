using System.Collections;
using TMPro;
using UnityEngine;

public class StatueVisualEffect : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text _saveText;

    [Header("Model")]
    [SerializeField]
    private Transform _model;
    [SerializeField]
    private Transform _eyes;

    [Header("Particle")]
    [SerializeField]
    private ParticleSystem[] _particles;

    [Header("Sound")]
    [SerializeField]
    private AudioSource _saveSound;

    [Header("Timer")]
    [SerializeField]
    private float _minTextDisplayTime = 1f;
    private float _textDisplayTime = 0f;
    [SerializeField]
    private float _minParticlePlayTime = 2f;
    private float _particleStartTime = 0f;

    [Header("Preserve State")]
    [SerializeField]
    private bool Played = false;
    [SerializeField]
    PreserveState _statePreserver;

    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        if(_statePreserver)
        {
            _statePreserver.SaveState("_played", Played);
        }
    }

    private void Init()
    {
        _statePreserver = GetComponent<PreserveState>();
        if (_statePreserver)
        {
            Played = _statePreserver.LoadState("_played", false);

            //�� �ε�� �̹� �÷��� �� ���� �ִ� ��� ���� �÷��� ���°�
            //�����Ǿ�� �ϴ� ����Ʈ
            if(Played)
            {
                ActiveEyes();
            }
        }

        _saveSound = GetComponent<AudioSource>();

        SaveAndLoader.OnSaveStarted += PlayEffectsOnSaveStarted;
        SaveAndLoader.OnSaveEnded += DeactiveSaveTextLogic;
    }

    private void PlayEffectsOnSaveStarted()
    {
        //���� 1ȸ�� �ǽõǴ� ������
        if(!Played)
        {
            PlayDustParticle();
            ActiveEyes();
        }

        ActiveSaveText();
        PlaySaveSound();
        Played = true;
    }

    private void ActiveEyes()
    {
        if (_eyes == null)
            return;

        SpriteRenderer leftEye = _eyes.Find("LeftEye").GetComponent<SpriteRenderer>();
        leftEye.enabled = true;

        SpriteRenderer rightEye = _eyes.Find("RightEye").GetComponent<SpriteRenderer>();
        rightEye.enabled = true;
    }

    #region Particle
    private void PlayDustParticle()
    {
        for(int i = 0; i <  _particles.Length; i++)
        {
            if(_particles[i] == null) continue;

            _particles[i].Play();
        }
        _particleStartTime = Time.time;
    }
    #endregion

    #region UI TEXT
    private void ActiveSaveText()
    {
        if (_saveText == null) return;

        _saveText.enabled = true;
        _textDisplayTime = Time.time;
    }

    private void DeactiveSaveTextLogic()
    {
        if (_saveText == null) return;

        float remain = Time.time - _textDisplayTime;
        remain = remain < _minTextDisplayTime ? _minTextDisplayTime - remain : 0;

        StopAllCoroutines();
        StartCoroutine(DeactiveSaveTextTimer(remain));  
    }

    private IEnumerator DeactiveSaveTextTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        _saveText.enabled = false;
    }
    #endregion

    #region Sound

    private void PlaySaveSound()
    {
        if( _saveSound == null) return;

        _saveSound.Play();
    }
    #endregion

}
