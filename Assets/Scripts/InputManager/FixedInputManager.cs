using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedInputManager : HappyTools.SingletonBehaviourFixed<FixedInputManager>
{
    [SerializeField] KeyCode _interactionKey = KeyCode.Return;

    public static bool InteractionKeyDown { get { return Input.GetKeyDown(Instance._interactionKey); } }
}
