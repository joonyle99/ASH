using UnityEngine;

/// <summary>
/// �÷��̾ �� ���� �޴� ������Ʈ�� �����Ǵ� ������Ʈ
/// </summary>
public class LightCapturer : MonoBehaviour
{
    // ���� �޴� ������Ʈ�� �����Ǵ� ������Ʈ
    [RequireInterface(typeof(ILightCaptureListener))]
    [SerializeField] private Object _lightCaptureListenerObject;

    private bool _isGettingLighted = false;             // ���� �����ӿ� ���� �ް� �ִ��� ����
    private bool _wasGettingLighted = false;            // ���� �����ӿ� ���� �ް� �־����� ����
    private LightSource _lastLightSource = null;

    private ILightCaptureListener _lightCaptureListener => _lightCaptureListenerObject as ILightCaptureListener;
    public bool IsGettingLighted => _isGettingLighted;

    private void LateUpdate()
    {
        // ���� �����ӿ� ���� �ް� �־�����, �̹� �����ӿ� ���� ���� �ʾ��� ��
        if (!_isGettingLighted && _wasGettingLighted)
        {
            if (_lightCaptureListener != null)
            {
                _lightCaptureListener.OnLightExit(this, _lastLightSource);
                _lastLightSource = null;
            }
        }

        // ���� �������� ���¸� ���� �������� ���·� ������, ���� �������� �ʱ�ȭ
        _wasGettingLighted = _isGettingLighted;
        _isGettingLighted = false;
    }

    /// <summary>
    /// LightSource�� �� ������Ʈ�� ���� ����� �� ȣ��Ǵ� �޼���
    /// </summary>
    /// <param name="lightSource"></param>
    public void OnLightHit(LightSource lightSource)
    {
        _isGettingLighted = true;

        if (_lightCaptureListener != null)
        {
            if (!_wasGettingLighted)
                _lightCaptureListener.OnLightEnter(this, lightSource);
            else
                _lightCaptureListener.OnLightStay(this, lightSource);
        }

        _lastLightSource = lightSource;
    }
}
