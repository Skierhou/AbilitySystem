using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBuff : AbilityBase, IAbilityBuff
{
    #region 字段/属性
    /// Data
    protected FAbilityBuffData buffData;
    /// Temp
    Coroutine cor_Buff;

    public float Duration { get; protected set; }

    public int Stack { get => buffData.stack; set => buffData.stack = value; }
    #endregion

    public override void InitAbility(AbilitySystemComponent abilitySystem, AbilityEditorData abilityEditorData)
    {
        base.InitAbility(abilitySystem, abilityEditorData);
        buffData.stack = 0;
        buffData.maxStack = abilityEditorData.maxStack;
        InitBuffData(abilityEditorData.Buff_Data, abilityEditorData.Buff_Modifiers, abilityEditorData.Buff_MotionModifiers);
    }

    #region IAbilityBuff接口实现
    public virtual void InitBuffData(Editor_FAbilityBuffData inBuffData, List<Editor_FModifierData> inModifierData, List<Editor_FMotionModifierData> inMotionModifierData)
    {
        buffData.intervals = inBuffData.interval;
        buffData.durations = inBuffData.duration;
        buffData.durationPolicy = inBuffData.durationPolicy;
        buffData.modifiers = new List<AbilityBuffModifiers>();
        if (inModifierData != null)
        {
            foreach (Editor_FModifierData data in inModifierData)
            {
                AbilityBuffModifiers buffModifiers = new AbilityBuffModifiers(abilitySystem, data);
                buffData.modifiers.Add(buffModifiers);
            }
        }
        if (inMotionModifierData != null)
        {
            foreach (Editor_FMotionModifierData data in inMotionModifierData)
            {
                AbilityBuffMotionModifiers buffModifiers = new AbilityBuffMotionModifiers(abilitySystem, data);
                buffData.modifiers.Add(buffModifiers);
            }
        }
    }
    public override bool TryActivateAbility(AbilitySystemComponent inTargetSystem = null)
    {
        if (CanActivateAbility())
        {
            ActivateBuff();
            return true;
        }
        return false;
    }
    public override void EndAbility()
    {
        SetActive(false);
        if (cor_Buff != null)
        {
            abilitySystem.StopCoroutine(cor_Buff);
            cor_Buff = null;
        }
        abilitySystem.OnEndAbility(this);
        OnBuffEnd();
    }
    public override bool CanActivateAbility()
    {
        return !IsActive;
    }
    public virtual bool CanApplyModifier()
    {
        foreach (AbilityBuffModifiers modifier in buffData.modifiers)
        {
            if (!modifier.CanApplyModifier(Level))
                return false;
        }
        return true;
    }
    public virtual void UpdateLevelAndStack(int inLevel = 1, int inStackDelta = 1)
    {
        Level = Mathf.Min(Mathf.Max(Level, inLevel), MaxLevel);
        Stack += inStackDelta;

        if (Stack <= 0)
        {
            EndAbility();
        }
        else
        {
            if (inStackDelta > 0 && IsActive)
            {
                if (cor_Buff != null)
                {
                    abilitySystem.StopCoroutine(cor_Buff);
                    cor_Buff = null;
                }
                OnBuffEnd();
                ActivateBuff();
            }
        }
    }
    #endregion

    #region Buff执行接口实现
    protected virtual void ActivateBuff()
    {
        SetActive(true);
        OnBuffStart();
        switch (buffData.durationPolicy)
        {
            case EDurationPolicy.EDP_Instant:
                OnBuff();
                EndAbility();
                break;
            case EDurationPolicy.EDP_Infinite:
                cor_Buff = abilitySystem.StartCoroutine(Buff(float.MaxValue));
                break;
            case EDurationPolicy.EDP_HasDuration:
                cor_Buff = abilitySystem.StartCoroutine(Buff(buffData.durations[Level]));
                break;
        }
    }
    protected virtual void SetActive(bool inActive)
    {
        if (IsActive == inActive) return;

        IsActive = inActive;
        if (IsActive)
        {
            abilitySystem.AddBlockTags(blockAbilitiesWithTags);
            abilitySystem.AddActivateTags(abilityTags);
            abilitySystem.AddActivateTags(activationOwnedTags);
        }
        else
        {
            abilitySystem.RemoveBlockTags(blockAbilitiesWithTags);
            abilitySystem.RemoveActivateTags(abilityTags);
            abilitySystem.RemoveActivateTags(activationOwnedTags);
        }
    }
    protected virtual IEnumerator Buff(float inDuration)
    {
        float timer = buffData.bActiveFirst ? 0.0f : buffData.intervals[Level];
        Duration = inDuration;
        while (Duration > 0.0f)
        {
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = buffData.intervals[Level];
                OnBuff();
            }
            Duration -= Time.deltaTime;
            yield return null;
        }
        EndAbility();
        cor_Buff = null;
    }
    protected virtual void OnBuffStart() { }
    protected virtual void OnBuff() 
    {
        ApplyModifiers();
    }
    protected virtual void OnBuffEnd() { }
    protected virtual void ApplyModifiers()
    {
        abilitySystem.CancelAbilityByOther(this);
        if (buffData.modifiers != null)
        {
            foreach (AbilityBuffModifiers modifier in buffData.modifiers)
            {
                modifier.ApplyModifier(Level);
            }
        }
    }
    #endregion
}