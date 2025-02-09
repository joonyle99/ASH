using UnityEngine;

public class SoundPlayer : SoundList
{
    public void PlayFirstElementSoundData_SFX()
    {
        if(Datas.Count == 0)
        {
            Debug.Log("Any sound data not exist");
            return;
        }

        PlaySFX(Datas[0].Key);
    }
}
