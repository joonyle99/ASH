using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scene Data", menuName = "LevelManagement/Scene Data")]
public class SceneData : ScriptableObject
{
    [SerializeField] Tymski.SceneReference _sceneReference;

}