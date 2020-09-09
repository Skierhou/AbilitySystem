using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FAbilityData
{
    public int level;

    //ability
    public EAbilityType abilityType;
    public ETargetType targetType;

    //是否在冷却
    public bool bCooldown;
    public float coolDown;
    public Action<float, float> OnCoolDownDeletage;

    //是否强制释放
    public bool bImmediately;
    public bool bIsCancelable;
    public bool bIsBlockingOtherAbilities;
    //OnSpell回调时间点
    public float castPoint;
    public float totalTime;

    public float channelStartTime;
    public float channelInterval;
    public float channelEndTime;
};

public interface IAbility
{
    AbilitySystemComponent GetSourceAbilitySystemComponent();

    //-------------------- ability call back interface -----------------------//
    //void OnAbilityInit();
    ///// 普通技能回调
    //void OnAbilityStart();
    //void OnSpell();
    //void OnAbilityFinish();
    ///// 引导类技能回调
    //void OnChannelStart();
    //void OnChannelThink();
    //void OnChannelEnd();
    ///// 开关类技能回调
    //void OnAbilityToggleOn();
    //void OnAbilityToggleOff();

    ///// 被动技能回调
    //void OnAbilityPassiveInit();

    //-------------------- user interface -----------------------//
    void CommitCooldown();
    void CommitCost();
    void CommitAbility();
    // 打断
    void CancelAbility();
    // 主动结束
    void EndAbility();

    bool CanActivateAbility();
    bool TryActivateAbility();
    bool CanCost();
}

public class Ability: AbilityBase,IAbility
{
    //技能数据
    public FAbilityData abilityData;

    protected AbilitySystemComponent abilitySystem;

    // 开关
    bool bIsActive;

    // 通用
    public AbilityBuff effect_CoolDown;
    public AbilityBuff effect_Cost;

    //临时
    Coroutine cor_Skill;

    public Ability(FAbilityData data,AbilitySystemComponent abilitySystem)
    {
        this.abilityData = data;
        this.abilitySystem = abilitySystem;
        OnAbilityInit();
    }
    public void CommitCooldown()
    {
        effect_CoolDown?.ActivateBuff();
    }
    public void CommitCost() 
    {
        effect_Cost?.ActivateBuff();
    }
    public void CommitAbility()
    {
        CommitCooldown();
        CommitCost();
    }
    protected void SetSkillActive(bool inActive)
    {
        bIsActive = inActive;
        if (bIsActive)
        {
            if(abilityData.bIsBlockingOtherAbilities)
                abilitySystem.AddBlockTags(blockAbilitiesWithTags);
            abilitySystem.AddActivateTags(abilityTags);
            abilitySystem.AddActivateTags(activationOwnedTags);
        }
        else
        {
            if(abilityData.bIsBlockingOtherAbilities)
                abilitySystem.RemoveBlockTags(blockAbilitiesWithTags);
            abilitySystem.RemoveActivateTags(abilityTags);
            abilitySystem.RemoveActivateTags(activationOwnedTags);
        }
    }

    public bool CanActivateAbility() 
    {
        return !bIsActive && (effect_CoolDown != null ? !effect_CoolDown.IsAcitve() : true) && CanCost();
    }
    public bool CanCost()
    {
        return effect_Cost != null ? effect_Cost.CanApplyModifier() : true;
    }
    public void CancelAbility() 
    {
        if(abilityData.bIsCancelable)
            EndAbility();
    }
    public void EndAbility()
    {
        if (bIsActive)
        {
            SetSkillActive(false);
            if (cor_Skill != null)
            {
                abilitySystem.StopCoroutine(cor_Skill);
                cor_Skill = null;
            }
            OnAbilityEnd();
            abilitySystem.OnEndAbility(this);
        }
    }
    public bool TryActivateAbility() 
    {
        if (CanActivateAbility())
        {
            if (abilityData.abilityType == (EAbilityType.EAT_PassiveAblity | EAbilityType.EAT_ToggleAbility))
            {
                SetSkillActive(true);
                OnAbilityStart();
            }
            else
            {
                cor_Skill = abilitySystem.StartCoroutine(Skill());
            }
            return true;
        }
        return false;
    }

    IEnumerator Skill()
    {
        bool bSelect = true;
        if (abilityData.targetType != ETargetType.ETT_None)
        {
            //等待选择目标
            while (IsWaitingSelectTarget())
            {
                yield return null;
            }
            bSelect = IsSelectTargetSuccess();
        }
        if (bSelect)
        {
            SetSkillActive(true);
            OnAbilityStart();
            switch (abilityData.abilityType)
            {
                case EAbilityType.EAT_GeneralAbility:
                    yield return new WaitForSeconds(abilityData.castPoint);
                    OnAbilitySpell();
                    yield return new WaitForSeconds(abilityData.totalTime - abilityData.castPoint);
                    EndAbility();
                    break;
                case EAbilityType.EAT_ChannelAbility:
                    yield return new WaitForSeconds(abilityData.channelStartTime);
                    OnChannelStart();
                    float tTimer = abilityData.channelEndTime - abilityData.channelStartTime;
                    float tInterval = abilityData.channelInterval;
                    while (tTimer > 0)
                    {
                        yield return null;
                        if (tInterval > 0)
                            tInterval -= Time.deltaTime;
                        else
                        {
                            OnAbilitySpell();
                            tInterval = abilityData.channelInterval;
                        }
                        tTimer -= Time.deltaTime;
                    }
                    OnChannelEnd();
                    yield return new WaitForSeconds(abilityData.totalTime - abilityData.channelEndTime);
                    EndAbility();
                    break;
            }
        }
        yield return null;
    }

    public AbilitySystemComponent GetSourceAbilitySystemComponent()
    {
        return abilitySystem;
    }

    /// <summary>
    /// 等待选择目标
    /// </summary>
    protected virtual bool IsWaitingSelectTarget() { return true; }
    /// <summary>
    /// 选择目标是否成功
    /// </summary>
    protected virtual bool IsSelectTargetSuccess() { return true; }
    protected virtual void OnAbilityInit() { }
    protected virtual void OnAbilityStart() { }
    protected virtual void OnAbilitySpell() { }
    protected virtual void OnAbilityEnd() { }
    /// <summary>
    /// 引导类技能开启
    /// </summary>
    protected virtual void OnChannelStart() { }
    /// <summary>
    /// 引导类技能关闭
    /// </summary>
    protected virtual void OnChannelEnd() { }
}
