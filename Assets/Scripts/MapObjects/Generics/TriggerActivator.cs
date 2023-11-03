using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivatorType
{
    Player, Monster, Gimmick
}
public class TriggerActivator : MonoBehaviour
{
    ActivatorType _type;
    public ActivatorType Type => _type;
}
