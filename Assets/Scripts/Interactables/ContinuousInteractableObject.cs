using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContinuousInteractableObject : InteractableObject
{
    public abstract void InteractEnter();
    public abstract void InteractUpdate();
    public abstract void InteractExit();
}
