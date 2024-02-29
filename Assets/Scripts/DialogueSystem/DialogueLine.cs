
public class DialogueLine
{
    public float CharShowInterval { get { return 1 / CharactersPerSecond; } }

    public string Speaker = "";
    public string Text = "";
    public float CharactersPerSecond = 1.0f;
    public TextShakeParams ShakeParams;
    public DialogueAction Action;
}