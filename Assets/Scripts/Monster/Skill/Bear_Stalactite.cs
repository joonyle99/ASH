using UnityEngine;

public class Bear_Stalactite : Monster_SkillObject
{
    [SerializeField] SoundClipData _colideSound;
    private void OnDestroy()
    {
        SoundManager.Instance.PlaySFX(_colideSound);
    }
}
