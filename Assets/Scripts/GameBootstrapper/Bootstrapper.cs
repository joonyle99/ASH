public class Bootstrapper : HappyTools.GameBootstrapper
{
    public override void InitGame()
    {
        InputManager.Instance.ChangeToDefaultSetter();
        //SaveDataManager.Instance.Init();
    }
}
