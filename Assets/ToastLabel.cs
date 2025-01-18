using UnityEngine;

public class ToastLabel : MonoBehaviour
{
    private string _message;
    private float _timer;

    private Vector2 _startPosition;
    private Vector2 _currentPosition;

    private Color _textColor;

    private void OnGUI()
    {
        if (_timer > 0)
        {
            // GUIStyle 설정
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = _textColor;

            // 메시지를 화면에 표시
            GUI.Label(new Rect(_currentPosition.x, _currentPosition.y, 200, 50), _message, style);

            // 시간에 따라 위치와 투명도 변경
            float elapsed = Time.deltaTime;
            _timer -= elapsed;

            // 서서히 위로 이동
            _currentPosition.y -= elapsed * 50f; // 초당 50픽셀씩 이동

            // 서서히 투명해짐
            _textColor.a = Mathf.Clamp01(_timer / 2.0f); // 2초 동안 점점 투명해짐
        }  
    }

    public void ShowToast(string newMessage, float duration, Color color)
    {
        _message = newMessage;
        _timer = duration;

        _startPosition = new Vector2(Screen.width / 4 * 3, Screen.height / 4 * 3);
        _currentPosition = _startPosition;

        _textColor = color;
    }
}
