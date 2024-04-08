using UnityEngine;

public class CutscenePlayer_Event : MonoBehaviour
{
    public bool hasExecutedCutscene;
    public CutscenePlayer cutScene;

    public void Play()
    {
        if (!hasExecutedCutscene && cutScene)
        {
            hasExecutedCutscene = true;
            cutScene.Play();
        }
    }
}
