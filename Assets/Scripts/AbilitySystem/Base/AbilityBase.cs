using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

public abstract class AbilityBase
{
    protected AbilitySystemComponent abilitySystem;
    public AbilitySystemComponent AbilitySystemComponent { get { return abilitySystem; } }

    // 等级
    public int MaxLevel { get; protected set; }
    public int Level { get; protected set; }

    // 开关
    public bool IsActive { get; protected set; }

    // 是否强制释放
    public bool IsImmediately { get; protected set; }

    // 是否可被打断
    public bool IsCancelable { get; protected set; }

    // 是否阻挡其他能力
    public bool IsBlockingOtherAbilities { get; protected set; }

    // ----------------------------------------------------------------------------------------------------------------
    //	Ability exclusion / canceling
    // ----------------------------------------------------------------------------------------------------------------
    /** Self Tags，to check ability can block and cancel */
    public FAbilityTagContainer abilityTags;
    /** Abilities with these tags are cancelled when this ability is executed */
    public List<FAbilityTagContainer> cancelAbilitiesWithTags;
    /** Abilities with these tags are blocked while this ability is active*/
    public List<FAbilityTagContainer> blockAbilitiesWithTags;
    /** Tags to apply to activating owner while this ability is active */
    public List<FAbilityTagContainer> activationOwnedTags;
    /** This ability can only be source if the activating actor/component has all of these tags */
    public List<FAbilityTagContainer> sourceRequiredTags;
    /** This ability is blocked if the source actor/component has any of these tags */
    public List<FAbilityTagContainer> sourceBlockedTags;
    /** This ability can only be target if the activating actor/component has all of these tags */
    public List<FAbilityTagContainer> targetRequiredTags;
    /** This ability is blocked if the target actor/component has any of these tags */
    public List<FAbilityTagContainer> targetBlockedTags;

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void InitAbility(AbilitySystemComponent abilitySystem,AbilityEditorData abilityEditorData)
    {
        this.abilitySystem = abilitySystem;

        abilityTags = AbilityTagManager.Instance.GetTagContainer(abilityEditorData.abilityTags[0]);
        InitTagList(ref cancelAbilitiesWithTags,abilityEditorData.cancelAbilitiesWithTags);
        InitTagList(ref blockAbilitiesWithTags, abilityEditorData.blockAbilitiesWithTags);
        InitTagList(ref activationOwnedTags, abilityEditorData.activationOwnedTags);
        InitTagList(ref sourceRequiredTags, abilityEditorData.sourceRequiredTags);
        InitTagList(ref sourceBlockedTags, abilityEditorData.sourceBlockedTags);
        InitTagList(ref targetRequiredTags, abilityEditorData.targetRequiredTags);
        InitTagList(ref targetBlockedTags, abilityEditorData.targetBlockedTags);

        Level = 0;
        MaxLevel = abilityEditorData.maxLevel;
        IsActive = false;
        IsImmediately = abilityEditorData.bImmediately;
        IsCancelable = abilityEditorData.bIsCancelable;
        IsBlockingOtherAbilities = abilityEditorData.bIsBlockingOtherAbilities;

        AbilityEditorData.ReadChildInfo(GetType(), abilityEditorData, this);
    }
    /// <summary>
    /// 删除回收操作
    /// </summary>
    public virtual void DestroyAbility()
    {
        
    }
    /// <summary>
    /// 是否可以激活
    /// </summary>
    public abstract bool CanActivateAbility();
    /// <summary>
    /// 激活能力
    /// </summary>
    public abstract bool TryActivateAbility(AbilitySystemComponent inTargetAbilitySystem = null);
    /// <summary>
    /// 打断能力
    /// </summary>
    public virtual void CancelAbility()
    {
        if (IsCancelable)
            EndAbility();
    }
    /// <summary>
    /// 结束能力
    /// </summary>
    public virtual void EndAbility()
    {
        if (IsActive)
        {
            abilitySystem.OnEndAbility(this);
        }
    }

    protected void InitTagList(ref List<FAbilityTagContainer> refList, List<string> strList)
    {
        refList = new List<FAbilityTagContainer>();
        foreach (string str in strList)
        {
            refList.Add(AbilityTagManager.Instance.GetTagContainer(str));
        }
    }
}
