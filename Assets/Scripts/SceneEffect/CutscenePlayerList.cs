using System.Collections.Generic;
using UnityEngine;

public class CutscenePlayerList : MonoBehaviour
{
    [SerializeField]
    private CutscenePlayer[] _cutscenePlayers;
    private Queue<CutscenePlayer> _cutscenePlayerQueue;

    private void Awake()
    {
        if (_cutscenePlayers.Length == 0)
            Debug.LogError("No cutscene player to play");

        // 배열을 바로 Queue의 생성자에 넘겨주는 것이 더 직관적
        _cutscenePlayerQueue = new Queue<CutscenePlayer>(_cutscenePlayers);
    }

    public void PlayNextCutScene()
    {
        if (_cutscenePlayerQueue.Count == 0)
            return;

        var cutscenePlayer = _cutscenePlayerQueue.Dequeue();
        cutscenePlayer.Play();
    }

    public void PassNextCutScene()
    {
        if (_cutscenePlayerQueue.Count == 0)
            return;

        _cutscenePlayerQueue.Dequeue();
    }

    public void PlayCutscene(int index)
    {
        if(index < 0 || index >= _cutscenePlayers.Length)
            return;

        _cutscenePlayers[index].Play();
    }
}
