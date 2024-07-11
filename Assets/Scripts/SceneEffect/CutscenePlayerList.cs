using UnityEngine;

[System.Serializable]
public class CutsceneDictionary
{
    public string name;
    public CutscenePlayer cutscenePlayer;
}

public class CutscenePlayerList : MonoBehaviour
{
    [SerializeField]
    private CutsceneDictionary[] _cutsceneDictionary;

    public void PlayCutscene(string cutsceneName)
    {
        var cutscenePlayer = CheckPlayed(cutsceneName);

        if (cutscenePlayer == null)
        {
            Debug.LogWarning("Cutscene is invalid: " + cutsceneName);
            return;
        }

        cutscenePlayer.Play();
    }
    public CutscenePlayer CheckPlayed(string cutsceneName)
    {
        var cutscenePlayer = FindCutscene(cutsceneName);

        if (cutscenePlayer == null)
        {
            Debug.LogWarning("Cutscene not found: " + cutsceneName);
            return null;
        }

        return cutscenePlayer.IsPlayed ? null : cutscenePlayer;
    }
    public CutscenePlayer FindCutscene(string cutsceneName)
    {
        // Find the cutscene player with the given name
        foreach (CutsceneDictionary cutscene in _cutsceneDictionary)
        {
            if (cutscene.name == cutsceneName)
                return cutscene.cutscenePlayer;
        }

        return null;
    }

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
    public static bool CheckAnyPlayingInScene()
    {
        // 씬에서 모든 cutsceneplayer를 찾는다

        var allCutscenePlayer = FindObjectsByType<CutscenePlayer>(FindObjectsSortMode.None);
        foreach (var cutscene in allCutscenePlayer)
        {
            if (cutscene.IsPlaying)
            {
                return true;
            }
        }

        return false;
    }
}
