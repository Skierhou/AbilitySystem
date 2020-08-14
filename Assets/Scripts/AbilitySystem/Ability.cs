using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAbilityType
{
    EAT_PassiveAblity,
    EAT_GeneralAbility,
    EAT_ChannelAbility,
    EAT_ToggleAbility,
    EAT_ActivateAbility,
};
public enum ETargetType
{
    ETT_None,
    ETT_Target,
    ETT_Ground,
};
public enum ECostType
{
    ECT_None,
    ECT_Mana,
    ECT_Health,
};

public class Ability
{
    private EAbilityType abilityType;
    private ETargetType targetType;
    private ECostType costType;
    private float costAmount;

    //是否强制释放
    bool bImmediately = false;
    //OnSpell回调时间点
    float CastPoint;
    //是否在冷却
    bool bCooldown;
    float CoolDown;

    AbilitySystemComponent abilitySystem;

    public Ability(AbilitySystemComponent abilitySystem)
    {
        this.abilitySystem = abilitySystem;
    }

    public virtual void OnAbilityInit() { }

    /// 普通技能回调
    public virtual void OnAbilityStart() { }
    public virtual void OnSpell() { }
    public virtual void OnAbilityFinish() { }
    /// 引导类技能回调
    public virtual void OnChannelStart() { }
    public virtual void OnChannelThink() { }
    public virtual void OnChannelEnd() { }
    /// 开关类技能回调
    public virtual void OnAbilityToggleOn() { }
    public virtual void OnAbilityToggleOff() { }
    /// 激活类技能回调
    public virtual void OnAbilityActivate() { }
    public virtual void OnAbilityDeactivate() { }

    //************************************* 通用 **************************************//
    /// <summary>
    /// 冷却
    /// </summary>
    public void StartCooldown(float inDuration)
    {
        abilitySystem.StartCoroutine(Cor_Cooldown(inDuration));
    }
    IEnumerator Cor_Cooldown(float inDuration)
    {
        bCooldown = true;
        yield return new WaitForSeconds(inDuration);
        bCooldown = false;
    }

    public bool CheckCanUse()
    {
        return !bCooldown && CheckCost();
    }
    public bool CheckCost()
    {
        return false;
    }
}
