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
            // GUIStyle ����
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = _textColor;

            // �޽����� ȭ�鿡 ǥ��
            GUI.Label(new Rect(_currentPosition.x, _currentPosition.y, 200, 50), _message, style);

            // �ð��� ���� ��ġ�� ���� ����
            float elapsed = Time.deltaTime;
            _timer -= elapsed;

            // ������ ���� �̵�
            _currentPosition.y -= elapsed * 50f; // �ʴ� 50�ȼ��� �̵�

            // ������ ��������
            _textColor.a = Mathf.Clamp01(_timer / 2.0f); // 2�� ���� ���� ��������
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
