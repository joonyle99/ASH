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
    private float _particleStartTime = 0f;

    [Header("Preserve State")]
    [SerializeField]
    private bool Played = false;
    private Identifier _identifier;

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
    }
    private void Init()
    {
        _audioSource = GetComponent<AudioSource>();
        _soundList = GetComponent<SoundList>();
        _identifier = GetComponent<Identifier>();

        if (_identifier)
        {
            PersistentData pd = PersistentDataManager.Instance.PersistentData;

            if (pd.DataGroups != null &&
                pd.DataGroups.TryGetValue(_identifier.GroupName, out var value))
            {
                if (value.TryGetValue(_identifier.ID + "_played", out var alreadyPlayed))
                {
                    Played = (bool)alreadyPlayed;
                }

            }

            //씬 로드시 이미 플레이 된 적이 있는 경우 켜진 플레이 상태가
            //유지되어야 하는 이펙트
            if (Played)
            {
                ActiveEyes();
            }
        }

        if (!PersistentDataManager.HasDataGroup(_identifier.GroupName))
        {
            PersistentDataManager.TryAddDataGroup(_identifier.GroupName);
        }
    }

    public void PlayEffectsOnSaveStarted()
    {
        //최초 1회만 실시되는 로직들
        if (!Played)
        {
            PlayDustParticle();
            ActiveEyes();
        }

        ActiveSaveText();
        PlaySaveSound();
        Played = true;
        _identifier.SaveState("_played", true);
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
        for (int i = 0; i < _particles.Length; i++)
        {
            if (_particles[i] == null) continue;

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

    public void DeactiveSaveTextLogic()
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
        if (_soundList == null) return;

        if (Played)
        {
            string key = "SE_Point_Save";
            if (_soundList.Exists(key))
            {
                _soundList.PlaySFX(key, 1f, 5f);
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
