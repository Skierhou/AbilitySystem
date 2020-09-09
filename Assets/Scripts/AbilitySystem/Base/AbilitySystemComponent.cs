using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilitySystem
{
    AbilitySystemComponent GetAbilitySystemComponent();
};

public class AbilitySystemComponent : MonoBehaviour
{
    /* posses ability list */
    HashSet<Ability> abilities;

    Dictionary<FAbilityTagContainer, Ability> abilitiesMap;
    Dictionary<FAbilityTagContainer, AbilityBuff> buffsMap;

    /* current Tag */
    FAbilityTagCountContainer activationAbilityTagsContainer;
    FAbilityTagCountContainer blockAbilityTagsContainer;

    List<Ability> needRemoveList;

    private void Awake()
    {
        abilities = new HashSet<Ability>();
        needRemoveList = new List<Ability>();
        abilitiesMap = new Dictionary<FAbilityTagContainer, Ability>();
        buffsMap = new Dictionary<FAbilityTagContainer, AbilityBuff>();
    }

    private void Update()
    {
        if (needRemoveList.Count > 0)
        {
            Ability tempAbility = needRemoveList[0];
            needRemoveList.RemoveAt(0);
            abilities.Remove(tempAbility);
        }
    }

    /// <summary>
    /// 获得能力
    /// </summary>
    public virtual void AcquireAbility(Ability inAbility) 
    {
        if (!abilities.Contains(inAbility))
        {
            abilitiesMap.Add(inAbility.abilityTags, inAbility);
        }
    }
    /// <summary>
    /// 移除能力
    /// </summary>
    public virtual void RemoveAbility(Ability inAbility) 
    {
        abilitiesMap.Remove(inAbility.abilityTags);
    }

    /// <summary>
    /// 尝试触发一个能力
    /// </summary>
    public virtual bool TryActivateAbilityByTag(string inTag) 
    {
        if (AbilityTagManager.Instance.GetTagContainer(inTag, out FAbilityTagContainer inAbilityTagContainer))
        {
            if (abilitiesMap.TryGetValue(inAbilityTagContainer, out Ability ability))
                return TryActivateAbility(ability);
        }
        return false;
    }
    public virtual bool TryActivateAbility(Ability inAbility) 
    {
        if (CanActivateAbility(inAbility))
        {
            if (inAbility.TryActivateAbility())
            {
                // 移除cancel列表的技能
                CancelAbilityByTags(inAbility);

                abilities.Add(inAbility);
                return true;
            }
        }
        return false;
    }

    public virtual bool TryCancelAbility(FAbilityTagContainer inTagContainer)
    {
        if (abilitiesMap.TryGetValue(inTagContainer, out Ability ability))
        {
            ability.CancelAbility();
            return true;
        }
        return false;
    }

    public virtual bool CanActivateAbility(AbilityBase inAbility)
    {
        return inAbility is object && (!blockAbilityTagsContainer.HasBlockMatchingTags(inAbility.abilityTags) && CheckSourceTags(inAbility));
    }
    /// <summary>
    /// 移除cancel列表的技能
    /// </summary>
    public virtual void CancelAbilityByTags(AbilityBase inAbility)
    {
        if (inAbility.cancelAbilitiesWithTags == null || inAbility.cancelAbilitiesWithTags.Count == 0)
            return;

        foreach (FAbilityTagContainer tagContainer in inAbility.cancelAbilitiesWithTags)
        {
            if (activationAbilityTagsContainer.HasAnyMatchingTags(tagContainer))
            {
                foreach (Ability ability in abilities)
                {
                    if (ability is object && ability.abilityTags.HasAll(tagContainer))
                    {
                        ability.CancelAbility();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 检测当前作用源是否符合条件
    /// </summary>
    public virtual bool CheckSourceTags(AbilityBase inAbility)
    {
        return !activationAbilityTagsContainer.HasBlockMatchingTags(inAbility.sourceBlockedTags) && activationAbilityTagsContainer.HasAnyMatchingTags(inAbility.sourceRequiredTags);
    }

    /// <summary>
    /// 检测当前作用目标是否符合条件
    /// </summary>
    public virtual bool CheckTargetTags(AbilityBase inAbility)
    {
        return activationAbilityTagsContainer.HasAnyMatchingTags(inAbility.targetRequiredTags) && !activationAbilityTagsContainer.HasBlockMatchingTags(inAbility.targetBlockedTags);
    }
    public virtual void AddBuff(AbilityBuff inBuff)
    {
        buffsMap.Add(inBuff.abilityTags, inBuff);
    }
    public virtual void RemoveBuff(AbilityBuff inBuff)
    {
        buffsMap.Remove(inBuff.abilityTags);
    }
    public virtual void TryActivateBuffByTag(string inTag,int inLevel = 1,int inStackDelta = 1)
    {
        if (AbilityTagManager.Instance.GetTagContainer(inTag, out FAbilityTagContainer abilityTagContainer))
        {
            if (buffsMap.TryGetValue(abilityTagContainer, out AbilityBuff buff))
            {
                buff.UpdateLevelAndStack(inLevel, inStackDelta);
            }
            else
            {
                if (AbilityManager.Instance.GetBuffData(abilityTagContainer, out FAbilityBuffData data))
                {
                    AbilityBuff tempBuff = AbilityManager.Instance.CreateAblityBuff(abilityTagContainer);
                    tempBuff.Init(this,data);
                    tempBuff.UpdateLevelAndStack(inLevel, inStackDelta);
                    tempBuff.ActivateBuff();
                }
            }
        }
    }
    public void AddBlockTags(List<FAbilityTagContainer> inTags)
    {
        foreach (FAbilityTagContainer tag in inTags)
        {
            blockAbilityTagsContainer.AddTags(tag);
        }
    }
    public void RemoveBlockTags(List<FAbilityTagContainer> inTags)
    {
        foreach (FAbilityTagContainer tag in inTags)
        {
            blockAbilityTagsContainer.RemoveTags(tag);
        }
    }
    public void AddActivateTags(FAbilityTagContainer inTag)
    {
        activationAbilityTagsContainer.AddTags(inTag);
    }
    public void AddActivateTags(List<FAbilityTagContainer> inTags)
    {
        foreach (FAbilityTagContainer tag in inTags)
        {
            activationAbilityTagsContainer.AddTags(tag);
        }
    }
    public void RemoveActivateTags(FAbilityTagContainer inTag)
    {
        activationAbilityTagsContainer.RemoveTags(inTag);
    }
    public void RemoveActivateTags(List<FAbilityTagContainer> inTags)
    {
        foreach (FAbilityTagContainer tag in inTags)
        {
            activationAbilityTagsContainer.RemoveTags(tag);
        }
    }
    public virtual void OnEndAbility(Ability inAbility)
    {
        if (inAbility is object)
        {
            needRemoveList.Add(inAbility);
        }
    }
}
