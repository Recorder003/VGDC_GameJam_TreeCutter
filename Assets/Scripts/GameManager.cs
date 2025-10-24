using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // General game manager, holds stats other scripts can reference through GameManager.Instance.value

    public int playerLives = 3;
    public float gameSpeed = 1f;
    public float musicVolume = 0.5f;
    public float enemyHitFlashDur = 0.2f;

    public static GameManager Instance { get; private set; }
    public GameObject skillSelectionUI;
    public GameObject skillPrefab;

    public int skillsToChoose = 3;
    List<Sprite> skillSprites = new List<Sprite>();
    List<MonoBehaviour> skillScripts = new List<MonoBehaviour>();
    List<Skill> skillsToDisplay = new List<Skill>();

    public enum ScriptType
    {
        Passive,
        OneShot
    }

    public class Skill
    {
        public string skillName;
        public string skillImageName;
        public string skillDescription;
        public string skillScriptName; //the script belonging to the skill?
        public int skillRarity;
        public ScriptType scriptType;

   

        public Skill(string name, string skillImageName, string skillDescription, string skillScriptName, int skillRarity, ScriptType scriptType)
        {
            this.skillName = name;
            this.skillImageName = skillImageName;
            this.skillDescription = skillDescription;
            this.skillScriptName = skillScriptName;
            this.skillRarity = skillRarity;
            this.scriptType = scriptType;
        }
    }

    List<Skill> skills;



    private void Awake()
    {
        //set this to instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        skills = new List<Skill>();

        skills.Add(new Skill("Axe Multishot", "AxeImage", "Throw more axes", "AxeMulti", 1, ScriptType.OneShot));
        skills.Add(new Skill("Sharper Axe", "AxeImage", "Your axe is sharper...", "SharpAxe", 1, ScriptType.OneShot));
        skills.Add(new Skill("Axe Multishot TWO", "AxeImage", "Throw even more axes", "AxeMulti", 1, ScriptType.OneShot));


        Sprite[] Sprites;
        Sprites = Resources.LoadAll<Sprite>("SkillSprites");

        MonoBehaviour[] Scripts;
        Scripts = Resources.LoadAll<MonoBehaviour>("SkillScripts");

        for (int i = 0; i < Sprites.Length; i++)
        {
            skillSprites.Add(Sprites[i]);
        }


    }

    public void playerLeveledUp(int newLevel)
    {
        Time.timeScale = 0f;
        showSkillUI();

    }

    private void showSkillUI()
    {
        //bring up skill selection UI
        //generate new skills

        for (int i = 0; i < skillsToChoose; i++)
        {
            int randomSkillIndex = Random.Range(0, skills.Count);
            Skill randomSkill = skills[randomSkillIndex];

            GameObject skillObj = Instantiate(skillPrefab, skillSelectionUI.transform);
            skillObj.SetActive(true);
            SetUpSkill(randomSkill, skillObj);
            skillsToDisplay.Add(randomSkill);
        }

    }

    private void SetUpSkill(Skill aSkill, GameObject skillPrefab)
    {
        skillPrefab.transform.Find("SkillName").GetComponent<TextMeshProUGUI>().text = aSkill.skillName;
        skillPrefab.transform.Find("SkillDescription").GetComponent<TextMeshProUGUI>().text = aSkill.skillDescription;
        skillPrefab.transform.Find("SkillImage").GetComponent<Image>().sprite = skillSprites.Find(s => s.name == aSkill.skillImageName);



    }

    public void SkillChosen(GameObject skillObj)
    {

        string skillName = skillObj.GetComponent<TextMeshProUGUI>().text;

        foreach (Skill skill in skillsToDisplay)
        {
            if (skillName == skill.skillName)
            {
                //skillScripts.Add((MonoBehaviour)System.Activator.CreateInstance(System.Type.GetType(skill.skillScriptName))); //??
                MonoBehaviour myScript = Instantiate(skillScripts.Find(s => s.name == skill.skillName), gameObject.transform);
                Destroy(myScript);
                //should run awake then die

            }
        }

        skillsToDisplay.Clear();
        Time.timeScale = 1f;
        //destroy children in skillui

        for (int i = skillSelectionUI.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(skillSelectionUI.transform.GetChild(i).gameObject);
        }


    }


}
