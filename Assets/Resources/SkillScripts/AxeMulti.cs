using UnityEngine;

public class AxeMulti : SkillBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject KnifeControllerPrefab;
    
    public override void PerformSkill()
    {


        KnifeController.Instance.decreaseCooldown(0.5f);

    }

}
