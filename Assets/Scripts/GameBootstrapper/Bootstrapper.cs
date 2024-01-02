using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : HappyTools.GameBootstrapper
{
    public override void InitGame()
    {
        InputManager.Instance.ChangeToDefaultSetter();
        //SaveDataManager.Instance.Init();
    }

}
