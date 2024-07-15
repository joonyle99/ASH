using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CutsceneDictionary
{
    public string name;
    public CutscenePlayer cutscenePlayer;

    public CutsceneDictionary(string name, CutscenePlayer cutscenePlayer)
    {
        this.name = name;
        this.cutscenePlayer = cutscenePlayer;
    }
}

public class CutscenePlayerList : MonoBehaviour
{
    [SerializeField]
    private List<CutsceneDictionary> _cutsceneDictionary = new List<CutsceneDictionary>();

    // play
    public void PlayCutscene(string cutsceneName)
    {
        // ���� CutscenePlayerList ��ü�� cutsceneDictionary���� cutscenePlayer�� Ȯ��
        var cutscenePlayer = FindCutscene(cutsceneName);

        if (cutscenePlayer == null)
        {
            // ���� ������ cutscenePlayer�� ã�� cutsceneDictionary�� �߰��Ѵ�
            cutscenePlayer = FindCutsceneInScene(cutsceneName);

            if(cutscenePlayer == null)
            {
                Debug.Log($"Cutscene not found: {cutsceneName}");
                return;
            }
        }

        cutscenePlayer.Play();
    }

    // find
    public CutscenePlayer FindCutscene(string cutsceneName)
    {
        var cutscenePlayer = _cutsceneDictionary
            .FirstOrDefault(c => c.name == cutsceneName && !c.cutscenePlayer.IsPlayed)?.cutscenePlayer;

        if (cutscenePlayer == null)
        {
            // Debug.LogWarning($"Cutscene not found or already played: {cutsceneName}");
            return null;
        }

        return cutscenePlayer;
    }

    public CutscenePlayer FindCutsceneInScene(string cutsceneName)
    {
        var cutscenePlayer = FindObjectsByType<CutscenePlayer>(FindObjectsSortMode.None)
             .FirstOrDefault(c => c.name == cutsceneName && !c.IsPlayed);

        if (cutscenePlayer == null)
        {
            // Debug.LogWarning($"Cutscene not found or already played: {cutsceneName}");
            return null;
        }

        _cutsceneDictionary.Add(new CutsceneDictionary(cutsceneName, cutscenePlayer));
        return cutscenePlayer;
    }

    // check
    public bool CheckAnyPlaying()
    {
        foreach (CutsceneDictionary cutscene in _cutsceneDictionary)
        {
            if (cutscene.cutscenePlayer.IsPlaying)
            {
                return true;
            }
        }

        return false;
    }
}
