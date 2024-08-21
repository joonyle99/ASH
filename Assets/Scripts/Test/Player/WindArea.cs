using UnityEngine;

public class WindArea : MonoBehaviour, ISceneContextBuildListener
{
    [SerializeField] private bool _isStartActive = true;

    private SoundList _soundList;
    private PreserveState _statePreserver;

    private void Awake()
    {
        _soundList = GetComponent<SoundList>();
        _statePreserver = GetComponentInParent<PreserveState>();
    }

    public void OnSceneContextBuilt()
    {
        if (_statePreserver != null)
        {
            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                _isStartActive = _statePreserver.LoadState("_isActiveSaved", _isStartActive);
                gameObject.SetActive(_isStartActive);
            }
            else
            {
                _isStartActive = _statePreserver.LoadState("_isActive", _isStartActive);
                gameObject.SetActive(_isStartActive);
            }
        }

        _isStartActive = gameObject.activeSelf;

        SaveAndLoader.OnSaveStarted -= SaveWindState;
        SaveAndLoader.OnSaveStarted += SaveWindState;
    }

    private void OnDestroy()
    {
        if(_statePreserver)
        {
            if(SceneChangeManager.Instance && SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
            {
                _statePreserver.SaveState("_isActive", gameObject.activeSelf);
            }
        }

        SaveAndLoader.OnSaveStarted -= SaveWindState;
    }

    public void SetActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void SaveWindState()
    {
        if(_statePreserver)
        {
            _statePreserver.SaveState("_isActiveSaved", gameObject.activeSelf);
        }
    }
}
