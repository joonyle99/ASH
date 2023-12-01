using HappyTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAuthority : SingletonBehaviour<CameraAuthority>
{
    [SerializeField] CameraController _controller;

    public CameraController Controller { get { return _controller; } }
}
