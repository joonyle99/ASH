using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct DialogueScriptInfo
{
    public string Text;
    public float Speed;
    public ShakeInfo Shake;
}

public struct ShakeInfo
{
    public float RotationPower;
    public float MovePower;
    public float Speed;

    public ShakeInfo(float rotationPower = 1, float movePower = 1, float speed = 1)
    {
        RotationPower = rotationPower;
        MovePower = movePower;
        Speed = speed;
    }

    public static ShakeInfo None { get { return new ShakeInfo(0, 0, 0); } }
}