using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSkill", menuName = "Combat/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName = "New Skill";
    public int animatorIndex = 1;
    public float damageMultiplier = 1f;
    public float cooldown = 1f;

    public KeyCode inputKey; 
}
