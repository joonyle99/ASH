using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Passage Data", menuName ="LevelManagement/Passage Data")]
public class PassageData : ScriptableObject
{
    [SerializeField] Tymski.SceneReference _targetScene;
    [SerializeField] string _name;

    public string TargetSceneName { get { return _targetScene; } }
    public string Name { get { return _name; } }

}