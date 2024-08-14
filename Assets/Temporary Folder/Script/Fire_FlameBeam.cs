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
}
