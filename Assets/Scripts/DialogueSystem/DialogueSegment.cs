/// <summary>
/// 다이얼로그 세그먼트
/// </summary>
public class DialogueSegment
{
    public string Speaker = "";                                 // 발화자
    public string Text = "";                                    // 대사 스크립트
    public float CharactersPerSecond = 1.0f;                    // 1초에 나올 글자의 개수        
    public TextShakeParams ShakeParams;                         // 흔들림 효과
    public DialogueAction Action;                               // 실행할 액션

    public float CharShowInterval => 1 / CharactersPerSecond;   // 글자가 나타나는 간격
}