using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private AudioSource _audioSource;
    private SoundList _soundList;

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
    PreserveState _statePreserver;

    private void Awake()
    {
    }
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        _statePreserver = GetComponent<PreserveState>();
        if (_statePreserver)
        {
            JsonDataManager.JsonLoad();
            JsonPersistentData spd = JsonDataManager.GetObjectInGlobalSaveData<JsonPersistentData>("PersistentData");
            if(spd != null)
            {
                PersistentData pd = JsonPersistentData.ToNormalFormatClassObject(spd);
                string currentScene = SceneManager.GetActiveScene().name;
                if (pd._dataGroups != null &&
                    pd._dataGroups.TryGetValue(currentScene, out var value))
                {
                    if (value.TryGetValue(_statePreserver.EditorID + "_played", out var alreadyPlayed))
                    {
                        Played = (bool)alreadyPlayed;
                    }
                }
            }

            //�� �ε�� �̹� �÷��� �� ���� �ִ� ��� ���� �÷��� ���°�
            //�����Ǿ�� �ϴ� ����Ʈ
            if(Played)
            {
                ActiveEyes();
            }
        }

        _audioSource = GetComponent<AudioSource>();
        _soundList = GetComponent<SoundList>();

        SaveAndLoader.OnSaveStarted += PlayEffectsOnSaveStarted;
        SaveAndLoader.OnSaveEnded += DeactiveSaveTextLogic;
    }

    private void PlayEffectsOnSaveStarted()
    {
        //���� 1ȸ�� �ǽõǴ� ������
        if (!Played)
        {
            if (_statePreserver)
            {
                _statePreserver.SaveState("_played", true);
            }

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
        if(_soundList == null) return;
        
        if(Played)
        {
            string key = "SE_Point_Save";
            if (_soundList.Exists(key))
            {
                _soundList.PlaySFX(key, 5);
            }
            else
                Debug.Log("SE_Point_Save Audio Clip not found");
        }
        else
        {
            string key = "SE_Point_Statue";
            if (_soundList.Exists(key))
            {
                _soundList.PlaySFX(key);
            }
            else
                Debug.Log("SE_Point_Statue Audio Clip not found");
        }
    }
    #endregion

}
