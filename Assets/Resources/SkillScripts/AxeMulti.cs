using UnityEngine;

public class AxeMulti : SkillBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject KnifeControllerPrefab;
    
    public override void PerformSkill()
    {


        print("Axe Multi Skill Activated");
        KnifeController.Instance.decreaseCooldown(0.1f);

    }

}
