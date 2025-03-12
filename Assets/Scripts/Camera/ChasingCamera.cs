using UnityEngine;

public class ChasingCamera : MonoBehaviour
{
    [SerializeField] private Transform _targetTrans;
    [SerializeField] private Transform _helperTrans;

    [Space]

    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _distance = 1f;

    private bool _isChasing = false;

    private CameraController _camera;

    public float DeadLinePosY
    {
        get
        {
            if (!_isChasing)
            {
                return -10000000;
            }
            else
            {
                return _helperTrans.position.y;
            }
        }
    }

    private void Awake()
    {
        _camera = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (_isChasing)
        {
            // 2. HelperTrans -> TargetTrans
            var nextHelperPos = new Vector3
                (
                SceneContext.Current.Player.transform.position.x,
                Mathf.MoveTowards(_helperTrans.position.y, _targetTrans.position.y, _speed * Time.deltaTime),
                SceneContext.Current.Player.transform.position.z
                );
            _helperTrans.position = nextHelperPos;

            if (Mathf.Abs(_targetTrans.position.y - _helperTrans.position.y) <= _distance)
            {
                StopChasing();
                this.enabled = false;
                return;
            }
        }
    }
    private void LateUpdate()
    {
        if (_isChasing)
        {
            if (SceneEffectManager.Instance.IsPlayingCutscene) return;

            var player = SceneContext.Current.Player;

            // 플레이어가 카메라 안에 있는지 확인
            var playerViewportPos = SceneContext.Current.CameraController.MainCamera.WorldToViewportPoint(player.HeadTrans.position);
            if (playerViewportPos.y < 0 && !BossLantern.IsPlayingLostLantern)
            {
                //Debug.Log("플레이어가 카메라 아래로 떨어져서 사망합니다.");
                
                player.CurHp -= player.CurHp;
                EndChasing();
                return;
            }
            else if (playerViewportPos.y > 1)
            {
                //Debug.Log("플레이어가 카메라 위로 올라가서 따라갑니다.");
                
                _helperTrans.position = new Vector3
                    (
                    _helperTrans.position.x,
                    (_helperTrans.position.y + player.HeadTrans.position.y) / 2f,
                    _helperTrans.position.z
                    );
            }
        }
    }
    private void OnEnable()
    {
        //Debug.Log("Chasing Camera On !");

        if (SceneEffectManager.Instance != null)
        {
            SceneEffectManager.Instance.OnAdditionalBefore += StopChasing;
            SceneEffectManager.Instance.OnAdditionalAfter += StartChasing;
        }
    }
    private void OnDisable()
    {
        //Debug.Log("Chasing Camera Off !");

        if (SceneEffectManager.Instance != null)
        {
            SceneEffectManager.Instance.OnAdditionalBefore -= StopChasing;
            SceneEffectManager.Instance.OnAdditionalAfter -= StartChasing;
        }
    }

    public void StartChasing()
    {
        if (_isChasing == true)
            return;

        // Debug.Log("Start Chasing");

        _isChasing = true;

        // 1. Camera -> HelperTrans (Follow)
        _camera.CurrentCameraType = CameraController.CameraType.Chasing;
        _helperTrans.position = SceneContext.Current.Player.transform.position;
        SceneContext.Current.CameraController.StartFollow(_helperTrans.transform);
    }
    public void StopChasing()
    {
        if (_isChasing == false || BossLantern.IsPlayingLostLantern)
            return;

         //Debug.Log("Stop Chasing");

        _isChasing = false;

        // 3. Camera -> Player (Follow)
        _camera.CurrentCameraType = CameraController.CameraType.Normal;
        SceneContext.Current.CameraController.StartFollow(SceneContext.Current.Player.transform);
    }
    public void EndChasing()
    {
        if (_isChasing == false)
            return;

        _isChasing = false;

        _camera.CurrentCameraType = CameraController.CameraType.Normal;
    }
}
