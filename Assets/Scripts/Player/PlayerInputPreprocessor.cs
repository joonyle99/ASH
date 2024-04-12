using UnityEngine;

public class PlayerInputPreprocessor : MonoBehaviour
{
    [SerializeField] private float _inputAdaptSpeed = 6f;
    [SerializeField] private float _inputAdaptSpeedInAir = 6f;

    /// <summary>
    /// InputState.Movement.x�� 0~1�� Smooth �� InputState
    /// </summary>
    // public InputState SmoothedInputs => _smoothedInputs;

    private InputState _smoothedInputs;
    private PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        InputState inputs = InputManager.Instance.State;

        float adaptSpeed = _player.IsGrounded ? _inputAdaptSpeed : _inputAdaptSpeedInAir;
        if (_player.Rigidbody.velocity.x * inputs.Movement.x < 0) // ���� �ӵ��� �Է� ������ �ݴ��� �� �����ӵ� ����
            adaptSpeed *= 3;

        float movementX = Mathf.MoveTowards(_smoothedInputs.Movement.x, inputs.Movement.x, adaptSpeed * Time.deltaTime);

        _smoothedInputs = inputs;
        _smoothedInputs.Movement.x = movementX;
    }
}

// TODO : �ν����Ϳ� input / smoothed input ǥ��