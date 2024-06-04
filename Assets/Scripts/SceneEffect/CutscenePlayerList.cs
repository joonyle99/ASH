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
        var cutscenePlayer = FindCutscene(cutsceneName);

        if (cutscenePlayer == null)
        {
            Debug.LogWarning("Cutscene not found: " + cutsceneName);
            return;
        }

        cutscenePlayer.Play();
    }
    public void PlayCutscene(CutscenePlayer cutscenePlayer)
    {
        if (cutscenePlayer == null) return;
        cutscenePlayer.Play();
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
    public CutscenePlayer CheckPlayed(string cutsceneName)
    {
        var cutscenePlayer = FindCutscene(cutsceneName);

        if(cutscenePlayer == null)
        {
            Debug.LogWarning("Cutscene not found: " + cutsceneName);
            return null;
        }

        return cutscenePlayer.IsPlayed ? null : cutscenePlayer;
    }
}
