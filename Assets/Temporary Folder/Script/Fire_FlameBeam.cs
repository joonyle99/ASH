using UnityEngine;

public class Fire_FlameBeam : Monster_IndependentSkill
{
    public void ExecuteDissolveEffect()
    {
        GetComponent<Animator>().SetTrigger("Dissolve");
    }
    public void EnableCollider()
    {
        GetComponent<Collider2D>().enabled = true;
    }
    public void DestroyBeam()
    {
        Destroy(this.gameObject);
    }
    public void PlaySound_SE_Fire_Flamebeam1()
    {
        SoundManager.Instance.PlayCommonSFX("SE_Fire_Flamebeam1");
    }
    public void PlaySound_SE_Fire_Flamebeam2()
    {
        SoundManager.Instance.PlayCommonSFX("SE_Fire_Flamebeam2");
    }
}
