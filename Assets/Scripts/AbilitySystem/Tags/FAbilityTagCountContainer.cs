using System;
using System.Collections.Generic;
using UnityEngine.Events;


public struct FAbilityTagCountContainer
{
    public Dictionary<FAbilityTag, int> abilityTagCountMap;
    private Dictionary<FAbilityTag, UnityAction<FAbilityTag, int>> abilityTagEventMap;

    public void AddTags(FAbilityTagContainer inAbilityTagContainer)
    {
        if (inAbilityTagContainer.IsEmpty()) return;

        foreach (FAbilityTag abilityTag in inAbilityTagContainer.abilityTags)
        {
            AddTag(abilityTag);
        }
        //AddTag(inAbilityTagContainer.abilityTags[inAbilityTagContainer.abilityTags.Count - 1]);
    }
    public void AddTag(FAbilityTag inAbilityTag,int inNum = 1)
    {
        if (abilityTagCountMap == null)
            abilityTagCountMap = new Dictionary<FAbilityTag, int>();

        if (abilityTagCountMap.ContainsKey(inAbilityTag))
        {
            abilityTagCountMap[inAbilityTag] += inNum;
        }
        else
        {
            abilityTagCountMap.Add(inAbilityTag, inNum);
        }
    }
    public void RemoveTags(FAbilityTagContainer inAbilityTagContainer)
    {
        if (inAbilityTagContainer.IsEmpty()) return;

        foreach (FAbilityTag abilityTag in inAbilityTagContainer.abilityTags)
        {
            AddTag(abilityTag, -1);
        }
        //AddTag(inAbilityTagContainer.abilityTags[inAbilityTagContainer.abilityTags.Count - 1], -1);
    }
    public void UpdateTagRef(FAbilityTag inAbilityTag,int inCountDelta)
    {
        if (abilityTagCountMap.ContainsKey(inAbilityTag))
            abilityTagCountMap[inAbilityTag] += inCountDelta;
        else
            AddTag(inAbilityTag, inCountDelta);
    }
    public void ResgiterAbilityEvent(FAbilityTag inAbilityTag, UnityAction<FAbilityTag, int> inEvent)
    {
        if (!abilityTagEventMap.ContainsKey(inAbilityTag))
        {
            abilityTagEventMap.Add(inAbilityTag, inEvent);
        }
        else
        {
            abilityTagEventMap[inAbilityTag] += inEvent;
        }
    }
    public bool HasAnyMatchingTags(List<FAbilityTagContainer> inOtherContainers)
    {
        if (inOtherContainers == null || inOtherContainers.Count == 0) return true;

        foreach (FAbilityTagContainer tag in inOtherContainers)
        {
            if (!HasAnyMatchingTags(tag)) return false;
        }
        return true;
    }
    public bool HasAnyMatchingTags(FAbilityTagContainer inOtherContainer)
    {
        if (inOtherContainer.IsEmpty()) return true;

        foreach (FAbilityTag abilityTag in inOtherContainer.abilityTags)
        {
            if (HasMatchingTag(abilityTag)) return true;
        }
        return false;
    }
    public bool HasBlockMatchingTags(List<FAbilityTagContainer> inOtherContainers)
    {
        if (inOtherContainers == null || inOtherContainers.Count == 0) return false;

        foreach (FAbilityTagContainer tag in inOtherContainers)
        {
            if (!HasBlockMatchingTags(tag)) return false;
        }
        return true;
    }

    public bool HasBlockMatchingTags(FAbilityTagContainer inOtherContainer)
    {
        if (inOtherContainer.IsEmpty()) return false;

        foreach (FAbilityTag abilityTag in inOtherContainer.abilityTags)
        {
            if (HasMatchingTag(abilityTag)) return true;
        }
        return false;
    }
    public bool HasMatchingTag(FAbilityTag abilityTag)
    {
        if (abilityTagCountMap is null) return false;
        return abilityTagCountMap.ContainsKey(abilityTag) && abilityTagCountMap[abilityTag] > 0;
    }
}