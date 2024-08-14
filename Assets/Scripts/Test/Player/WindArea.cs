using UnityEngine;

public class WindArea : MonoBehaviour, ISceneContextBuildListener
{
    [SerializeField] bool _isStartActive = true;
    PreserveState _statePreserver;

    /*    [SerializeField] private bool _isWorking = false;
        public float value = 15f;
        private GameObject _player = null;
        [SerializeField] vector windVector;
        private Vector2 _vector;

        enum vector
        {
            up,
            down,
            right,
            left
        }*/

    private void Awake()
    {
        _statePreserver = GetComponentInParent<PreserveState>();
    }
    public void OnSceneContextBuilt()
    {
        if (_statePreserver != null)
        {
            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                _isStartActive = _statePreserver.LoadState("_isActiveSaved", _isStartActive);
            }
            else
            {
                _isStartActive = _statePreserver.LoadState("_isActive", _isStartActive);
            }
        }


        gameObject.SetActive(_isStartActive);

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

    /*    private void Start()
        {
            switch (windVector)
            {
                case vector.up:
                    _vector = Vector2.up;
                    break;
                case vector.down:
                    _vector = Vector2.down;
                    break;
                case vector.left:
                    _vector = Vector2.left;
                    break;
                case vector.right:
                    _vector = Vector2.right;
                    break;
                default:
                    _vector = Vector2.up;
                    break;
            }
        }


        private void FixedUpdate()
        {
            if (_isWorking)
            {
                _player.GetComponent<Rigidbody2D>().AddForce(_vector * value, ForceMode2D.Force);

                *//*            if (!_player.GetComponent<PlayerBehaviour>().CurrentStateIs<JumpState>())
                            {
                                _player.GetComponent<PlayerBehaviour>().ChangeState<JumpState>();
                            }*//*
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                _player = other.gameObject;
                _isWorking = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                _isWorking = false;
            }
        }*/

    private void SaveWindState()
    {
        if(_statePreserver)
        {
            _statePreserver.SaveState("_isActiveSaved", gameObject.activeSelf);
        }
    }
}
