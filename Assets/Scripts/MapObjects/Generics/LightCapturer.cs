using UnityEngine;

/// <summary>
/// 플레이어가 쏜 빛을 받는 오브젝트에 부착되는 컴포넌트
/// </summary>
public class LightCapturer : MonoBehaviour
{
    // 빛을 받는 오브젝트에 부착되는 컴포넌트
    [RequireInterface(typeof(ILightCaptureListener))]
    [SerializeField] private Object _lightCaptureListenerObject;

    private bool _isGettingLighted = false;             // 현재 프레임에 빛을 받고 있는지 여부
    private bool _wasGettingLighted = false;            // 이전 프레임에 빛을 받고 있었는지 여부
    private LightSource _lastLightSource = null;

    private ILightCaptureListener _lightCaptureListener => _lightCaptureListenerObject as ILightCaptureListener;
    public bool IsGettingLighted => _isGettingLighted;

    private void LateUpdate()
    {
        // 이전 프레임에 빛을 받고 있었지만, 이번 프레임에 빛을 받지 않았을 때
        if (!_isGettingLighted && _wasGettingLighted)
        {
            if (_lightCaptureListener != null)
            {
                _lightCaptureListener.OnLightExit(this, _lastLightSource);
                _lastLightSource = null;
            }
        }

        // 현재 프레임의 상태를 이전 프레임의 상태로 저장후, 현재 프레임은 초기화
        _wasGettingLighted = _isGettingLighted;
        _isGettingLighted = false;
    }

    /// <summary>
    /// LightSource가 이 오브젝트에 빛을 쏘았을 때 호출되는 메서드
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
