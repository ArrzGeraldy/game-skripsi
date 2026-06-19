using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public SkillData data;

    [HideInInspector]
    public float cooldownRemaining = 0f;

    public bool IsReady => cooldownRemaining <= 0f && data != null;

    public float CooldownPercent => data != null && data.cooldown > 0f
        ? Mathf.Clamp01(cooldownRemaining / data.cooldown)
        : 0f;

    public void StartCooldown()
    {
        if (data != null)
            cooldownRemaining = data.cooldown;
    }

    // func update cooldown
    public void Tick(float deltaTime)
    {
        if (cooldownRemaining > 0f)
            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - deltaTime);
    }
}
