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

            if (Mathf.Abs(_targetTrans.position.y - _helperTrans.position.y) < _distance)
            {
                StopChasing();
                this.enabled = false;
                return;
            }
        }
    }
    private void LateUpdate()
    {
        /*
        if (_isChasing)
        {
            var player = SceneContext.Current.Player;

            // �÷��̾ ī�޶� �ȿ� �ִ��� Ȯ��
            var playerViewportPos = SceneContext.Current.CameraController.MainCamera.WorldToViewportPoint(player.HeadTrans.position);
            if (playerViewportPos.x < 0 || playerViewportPos.x > 1 || playerViewportPos.y < 0)
            {
                Debug.Log("ī�޶� ������ �÷��̾ ������ ����մϴ�.");

                player.CurHp -= player.CurHp;

                if (_isChasing)
                    StopChasing();

                return;
            }
        }
        */
    }
    private void OnEnable()
    {
        Debug.Log("Chasing Camera On !");

        SceneEffectManager.Instance.OnAdditionalBefore -= StopChasing;
        SceneEffectManager.Instance.OnAdditionalBefore += StopChasing;
        SceneEffectManager.Instance.OnAdditionalAfter -= StartChasing;
        SceneEffectManager.Instance.OnAdditionalAfter += StartChasing;
    }
    private void OnDisable()
    {
        Debug.Log("Chasing Camera Off !");

        SceneEffectManager.Instance.OnAdditionalBefore -= StopChasing;
        SceneEffectManager.Instance.OnAdditionalAfter -= StartChasing;
    }

    public void StartChasing()
    {
        if (_isChasing == true)
            return;

        Debug.Log("Start Chasing");

        _isChasing = true;

        // 1. Camera -> HelperTrans (Follow)
        _camera.CurrentCameraType = CameraController.CameraType.Chasing;
        SceneContext.Current.CameraController.StartFollow(_helperTrans.transform);

        _helperTrans.position = SceneContext.Current.Player.transform.position;
    }
    public void StopChasing()
    {
        if (_isChasing == false)
            return;

        Debug.Log("Stop Chasing");

        _isChasing = false;

        // 3. Camera -> Player (Follow)
        _camera.CurrentCameraType = CameraController.CameraType.Normal;
        SceneContext.Current.CameraController.StartFollow(SceneContext.Current.Player.transform);
    }
}
