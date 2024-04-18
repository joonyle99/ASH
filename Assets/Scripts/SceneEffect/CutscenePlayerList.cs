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

    public void PlayCutscene(string name)
    {
        var cutscenePlayer = FindCutscene(name);

        if (cutscenePlayer != null)
            cutscenePlayer.Play();
        else
            Debug.LogWarning("Cutscene not found: " + name);
    }

    public CutscenePlayer FindCutscene(string name)
    {
        // Find the cutscene player with the given name
        foreach (CutsceneDictionary cutscene in _cutsceneDictionary)
        {
            if (cutscene.name == name)
                return cutscene.cutscenePlayer;
        }

        return null;
    }
}
