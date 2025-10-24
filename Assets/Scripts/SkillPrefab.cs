using TMPro;
using UnityEngine;

public class SkillPrefab : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void chosen(string ArbitraryName)
    {
        GameManager.Instance.SkillChosen(gameObject.transform.Find("SkillName").GetComponent<TextMeshProUGUI>().text);
    }

}
