
public interface ILightCaptureListener
{
    public void OnLightStay(LightCapturer capturer, LightSource lightSource) { }
    public void OnLightEnter(LightCapturer capturer, LightSource lightSource) { }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource) { }
}
