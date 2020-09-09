using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FAbilityBuffData
{
    public List<AbilityBuffModifiers> modifiers;
    public EDurationPolicy durationPolicy;

    public float duration;
    public float interval;

    public int level;
    public int stack;
};

public class AbilityBuff : AbilityBase
{
    AbilitySystemComponent abilitySystem;

    FAbilityBuffData buffData;

    private bool bActive;
    // 临时
    Coroutine cor_Buff;

    public int Level { get => buffData.level; private set => buffData.level = value; }
    public int Stack { get => buffData.stack; private set => buffData.stack = value; }
    protected virtual void OnBuffStart() { }
    protected virtual void OnBuff() { }
    protected virtual void OnBuffEnd() { }

    public void Init(AbilitySystemComponent inAbilitySystemComp, FAbilityBuffData inBuffData)
    {
        abilitySystem = inAbilitySystemComp;
        buffData = inBuffData;
    }

    public void ActivateBuff()
    {
        SetActive(true);
        OnBuffStart();
        switch (buffData.durationPolicy)
        {
            case EDurationPolicy.EDP_Instant:
                ApplyModifiers();
                EndBuff();
                break;
            case EDurationPolicy.EDP_Infinite:
                cor_Buff = abilitySystem.StartCoroutine(Buff(float.MaxValue));
                break;
            case EDurationPolicy.EDP_HasDuration:
                cor_Buff = abilitySystem.StartCoroutine(Buff(buffData.duration));
                break;
        }
    }
    public void EndBuff()
    {
        SetActive(false);
        abilitySystem.RemoveBuff(this);
        if (cor_Buff != null)
        {
            abilitySystem.StopCoroutine(cor_Buff);
            cor_Buff = null;
        }
        OnBuffEnd();
    }
    public void SetActive(bool inActive)
    {
        if (bActive == inActive) return;

        bActive = inActive;
        if (bActive)
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

    IEnumerator Buff(float inDuration)
    {
        float timer = buffData.interval;
        while (inDuration > 0.0f)
        {
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = buffData.interval;
                OnBuff();
            }
            inDuration -= Time.deltaTime;
            yield return null;
        }
        EndBuff();
        cor_Buff = null;
    }

    public bool IsAcitve()
    {
        return bActive;
    }

    public bool CanApplyModifier()
    {
        if (IsAcitve()) return false;

        //foreach (AbilityBuffModifiers modifier in buffData.modifiers)
        //{
        //    if (!modifier.CanApplyModifier(buffData.level))
        //        return false;
        //}
        return true;
    }

    private void ApplyModifiers()
    {
        //foreach (AbilityBuffModifiers modifier in buffData.modifiers)
        //{
        //    modifier.ApplyModifier(buffData.level);
        //}
    }

    public void UpdateLevelAndStack(int inLevel = 1,int inStackDelta = 1)
    {
        Level = Mathf.Max(Level, inLevel);
        Stack += inStackDelta;

        if (Stack <= 0)
        {
            EndBuff();
        }
        else
        {
            if (inStackDelta > 0 && bActive)
            {
                EndBuff();
                ActivateBuff();
            }
        }
    }
}