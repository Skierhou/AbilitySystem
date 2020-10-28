using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ESelectTarget
{
    ST_None,
    ST_WaitingSelect,
    ST_SelectSuccess,
    ST_SelectFail,
};

public class Ability: AbilityBase, IAbility
{
    #region 字段/属性
    /// 技能数据
    public FAbilityData abilityData;

    /// 通用
    public AbilityBuff effect_CoolDown;
    public AbilityBuff effect_Cost;

    public List<FAbilityTagContainer> passive_ListenerTags;
    public List<FAbilityTagContainer> triggerTags;

    /// Temp
    Coroutine cor_Skill;
    #endregion

    #region IAbility接口实现
    public override void InitAbility(AbilitySystemComponent abilitySystem, AbilityEditorData abilityEditorData)
    {
        base.InitAbility(abilitySystem, abilityEditorData);
        abilityData.abilityType = abilityEditorData.abilityType;
        abilityData.castPoint = abilityEditorData.castPoint;
        abilityData.channelStartTime = abilityEditorData.channelStartTime;
        abilityData.channelInterval = abilityEditorData.channelInterval;
        abilityData.channelEndTime = abilityEditorData.channelEndTime;
        abilityData.targetType = abilityEditorData.targetType;
        abilityData.totalTime = abilityEditorData.totalTime;
        if (abilityEditorData.Buff_CoolDown != null)
        {
            effect_CoolDown = AbilityManager.Instance.CreateAbility(AbilityTagManager.Instance.GetTagContainer(abilityEditorData.Buff_CoolDown.abilityTags[0]),abilitySystem) as AbilityBuff;
        }
        if (abilityEditorData.Buff_Cost != null)
        {
            effect_Cost = AbilityManager.Instance.CreateAbility(AbilityTagManager.Instance.GetTagContainer(abilityEditorData.Buff_Cost.abilityTags[0]),abilitySystem) as AbilityBuff;
        }
        InitTagList(ref passive_ListenerTags, abilityEditorData.passiveAbilityListenerTags);
        InitTagList(ref triggerTags, abilityEditorData.passiveAbilityTriggerTags);

        if (abilityData.abilityType == EAbilityType.EAT_PassiveAblity)
        {
            foreach (FAbilityTagContainer tag in passive_ListenerTags)
            {
                abilitySystem.RegisterEvent(tag, OnTriggerActivateAbility);
            }
        }
    }
    public override void DestroyAbility()
    {
        if (abilityData.abilityType == EAbilityType.EAT_PassiveAblity)
        {
            foreach (FAbilityTagContainer tag in passive_ListenerTags)
            {
                abilitySystem.RemoveEvent(tag, OnTriggerActivateAbility);
            }
        }
    }
    public virtual void CommitCooldown()
    {
        if (effect_CoolDown != null)
            abilitySystem.TryActivateBuff(effect_CoolDown, Level, 1, true);
    }
    public virtual void CommitCost() 
    {
        if (effect_Cost != null)
            abilitySystem.TryActivateBuff(effect_Cost, Level, 1, true);
    }
    public virtual void CommitAbility()
    {
        CommitCooldown();
        CommitCost();
    }
    public virtual bool CanCost()
    {
        return effect_Cost != null ? effect_Cost.CanActivateAbility() && effect_Cost.CanApplyModifier() : true;
    }
    public override bool TryActivateAbility(AbilitySystemComponent inTargetAbilitySystem = null)
    {
        if (abilityData.abilityType == EAbilityType.EAT_ToggleAbility)
        {
            ToggleTriggerTags(!IsActive);
            SetSkillActive(!IsActive);
            return true;
        }
        else if (CanActivateAbility())
        {
            ActivateAbility(inTargetAbilitySystem);
            return true;
        }
        return false;
    }
    public override bool CanActivateAbility()
    {
        return !IsActive && abilityData.selectTargetType == ESelectTarget.ST_None && (effect_CoolDown != null ? effect_CoolDown.CanActivateAbility() : true) && CanCost();
    }
    public override void EndAbility()
    {
        base.EndAbility();
        if (IsActive)
        {
            SetSkillActive(false);
            if (cor_Skill != null)
            {
                abilitySystem.StopCoroutine(cor_Skill);
                cor_Skill = null;
            }
            OnAbilityEnd();
            CommitAbility();
        }
    }
    #endregion

    #region 技能实现接口
    /// <summary>
    /// 被动触发技能
    /// </summary>
    protected virtual void OnTriggerActivateAbility(AbilitySystemComponent inTargetAbilitySystem = null)
    {
        if (CanActivateAbility())
        {
            OnAbilitySpell();
            ToggleTriggerTags(true);
        }
    }
    protected virtual void ToggleTriggerTags(bool bToggle)
    {
        foreach (FAbilityTagContainer tag in triggerTags)
        {
            if (bToggle)
            {
                if (!abilitySystem.TryActivateAbilityByTag(tag))
                {
                    abilitySystem.TryActivateBuffByTag(tag, Level);
                }
            }
            else
            {
                abilitySystem.TryCancelAbilityByTag(tag);
            }
        }
    }
    /// <summary>
    /// 激活技能
    /// </summary>
    protected virtual void ActivateAbility(AbilitySystemComponent inTargetAbilitySystem = null)
    {
        if (abilityData.abilityType == EAbilityType.EAT_PassiveAblity)
            OnTriggerActivateAbility(inTargetAbilitySystem);
        else
            cor_Skill = abilitySystem.StartCoroutine(Skill());
    }
    /// <summary>
    /// 设置技能启用状态
    /// </summary>
    protected virtual void SetSkillActive(bool inActive)
    {
        IsActive = inActive;
        if (IsActive)
        {
            abilitySystem.OnActivateAbilitySuccess(this);
            if (IsBlockingOtherAbilities)
                abilitySystem.AddBlockTags(blockAbilitiesWithTags);
            abilitySystem.AddActivateTags(abilityTags);
            abilitySystem.AddActivateTags(activationOwnedTags);
        }
        else
        {
            if (IsBlockingOtherAbilities)
                abilitySystem.RemoveBlockTags(blockAbilitiesWithTags);
            abilitySystem.RemoveActivateTags(abilityTags);
            abilitySystem.RemoveActivateTags(activationOwnedTags);
        }
    }
    /// <summary>
    /// 常规技能实现
    /// </summary>
    protected virtual IEnumerator Skill()
    {
        abilityData.selectTargetType = ESelectTarget.ST_None;
        if (abilityData.targetType != ETargetType.ETT_None)
        {
            //等待选择目标
            abilityData.selectTargetType = ESelectTarget.ST_WaitingSelect;
            abilitySystem.WaitingSelectTargetEvent(abilityData.targetType, abilityData.selectKeyCode, abilityData.unSelectKeyCode, OnSelectTarget);
            while (abilityData.selectTargetType == ESelectTarget.ST_WaitingSelect)
            {
                yield return null;
            }
        }
        if (abilityData.selectTargetType != ESelectTarget.ST_SelectFail)
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
        abilityData.selectTargetType = ESelectTarget.ST_None;
    }
    /// <summary>
    /// 选择成功或失败后回调
    /// </summary>
    protected virtual void OnSelectTarget(bool IsSelectSuc, Vector3 selectPoint, AbilitySystemComponent targetSystems = null)
    {
        abilityData.selectTargetType = IsSelectSuc ? ESelectTarget.ST_SelectSuccess : ESelectTarget.ST_SelectFail;
    }
    /// <summary>
    /// 技能开始
    /// </summary>
    protected virtual void OnAbilityStart() { }
    /// <summary>
    /// 技能释放
    /// </summary>
    protected virtual void OnAbilitySpell() { }
    /// <summary>
    /// 技能结束
    /// </summary>
    protected virtual void OnAbilityEnd() { }
    /// <summary>
    /// 引导类技能开启
    /// </summary>
    protected virtual void OnChannelStart() { }
    /// <summary>
    /// 引导类技能关闭
    /// </summary>
    protected virtual void OnChannelEnd() { }

    #endregion
}
