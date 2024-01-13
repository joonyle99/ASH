using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDataComponent : MonoBehaviour
{
    [SerializeField] string _cutsceneName;
    [SerializeField] List<SceneEffect> _sequence;

    public string CutsceneName { get { return  _cutsceneName; } }
    public List<SceneEffect> Sequence { get {  return _sequence; } }


}
