using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class AbilitySystemComponent : MonoBehaviour, IAbilitySystem
{
    /* posses ability list */
    HashSet<AbilityBase> activityAbilities;
    Dictionary<FAbilityTagContainer, AbilityBase> abilitiesMap;
    Dictionary<FAbilityTagContainer, AbilityBuff> buffMap;

    /* current Tag */
    FAbilityTagCountContainer activationAbilityTagsContainer;
    FAbilityTagCountContainer blockAbilityTagsContainer;

    /* Attribute */
    public AttributeSet AttributeSet { get; protected set; }
    public MovementComponent MovmentComponent { get; protected set; }

    /* Event */
    public Dictionary<FAbilityTagContainer, UnityAction<AbilitySystemComponent>> OnAbilityTriggerEventMap;

    /* Debug */
    [Header("Debug")]
    public bool bShowActivityAbility;
    public bool bShowBlockAbility;
    public bool bShowPossessAbility;
    public bool bShowAttribute;

    private void Awake()
    {
        AttributeSet = new AttributeSet();
        activityAbilities = new HashSet<AbilityBase>();
        abilitiesMap = new Dictionary<FAbilityTagContainer, AbilityBase>();
        buffMap = new Dictionary<FAbilityTagContainer, AbilityBuff>();
        OnAbilityTriggerEventMap = new Dictionary<FAbilityTagContainer, UnityAction<AbilitySystemComponent>>();

        MovmentComponent = GetComponent<MovementComponent>();

        AttributeSet.AddAttribute(EAttributeType.AT_Health, 100);
        AttributeSet.AddAttribute(EAttributeType.AT_Mana, 100);
    }

    private void OnGUI()
    {
        StringBuilder sb = new StringBuilder();
        float height = 20;
        if (bShowPossessAbility)
        {
            sb.Append("PossessAbility:");
            foreach (var item in abilitiesMap.Keys)
            {
                sb.Append(item + "|");
            }
            GUI.color = Color.black;
            GUI.Label(new Rect(20, height, 200, 50), sb.ToString());
            height += 50;
        }
        if (bShowAttribute)
        {
            foreach (var item in AttributeSet.AttributeMap.Keys)
            {
                string str = item + "=" + AttributeSet.AttributeMap[item].BaseValue + "/" + AttributeSet.AttributeMap[item].CurrentValue;
                GUI.Label(new Rect(20, height, 200, 20), str);
                height += 20;
            }
        }
        if (bShowActivityAbility)
        {
            sb.Clear();
            sb.Append("ActivityTags:");
            if (activationAbilityTagsContainer.abilityTagCountMap != null)
            {
                foreach (FAbilityTag tag in activationAbilityTagsContainer.abilityTagCountMap.Keys)
                {
                    if (activationAbilityTagsContainer.abilityTagCountMap[tag] > 0)
                        sb.Append(tag + "|");
                }
            }
            GUI.Label(new Rect(20, height, 200, 50), sb.ToString());
            height += 50;
        }
        if (bShowBlockAbility)
        {
            sb.Clear();
            sb.Append("BlockTags:");
            if (blockAbilityTagsContainer.abilityTagCountMap != null)
            {
                foreach (FAbilityTag tag in blockAbilityTagsContainer.abilityTagCountMap.Keys)
                {
                    if (blockAbilityTagsContainer.abilityTagCountMap[tag] > 0)
                        sb.Append(tag + "|");
                }
            }
            GUI.Label(new Rect(20, height, 200, 50), sb.ToString());
            height += 50;
        }
    }

    #region Ability
    public AbilityBase TryGetAbility(FAbilityTagContainer inTag)
    {
        AbilityBase abilityBase;
        if (!abilitiesMap.TryGetValue(inTag, out abilityBase) || abilityBase == null)
        {
            if (buffMap.TryGetValue(inTag, out AbilityBuff buff))
                abilityBase = buff;
        }
        return abilityBase;
    }
    public void AcquireAbilityByTag(string inTag)
    {
        if (AbilityTagManager.Instance.GetTagContainer(inTag, out FAbilityTagContainer tag))
        {
            AcquireAbilityByTag(tag);
        }
    }
    public void AcquireAbilityByTag(FAbilityTagContainer inTag)
    {
        if (!abilitiesMap.ContainsKey(inTag))
        {
            AbilityBase ability = AbilityManager.Instance.CreateAbility(inTag, this);
            AcquireAbility(ability);
        }
    }
    public void RemoveAbilityByTag(string inTag)
    {
        if (AbilityTagManager.Instance.GetTagContainer(inTag, out FAbilityTagContainer tag))
        {
            RemoveAbilityByTag(tag);
        }
    }
    public void RemoveAbilityByTag(FAbilityTagContainer inTag)
    {
        if (abilitiesMap.TryGetValue(inTag, out AbilityBase ability))
        {
            RemoveAbility(ability);
        }
    }
    public virtual void AcquireAbility(AbilityBase inAbility)
    {
        if (inAbility is null) return;
        if (!abilitiesMap.ContainsKey(inAbility.abilityTags))
        {
            abilitiesMap.Add(inAbility.abilityTags, inAbility);
        }
    }
    public virtual void RemoveAbility(AbilityBase inAbility)
    {
        if (activityAbilities.Contains(inAbility))
            inAbility.CancelAbility();
        abilitiesMap.Remove(inAbility.abilityTags);
        AbilityManager.Instance.DestroyAbility(inAbility);
    }
    public bool TryActivateAbilityByTag(string inTag)
    {
        if (AbilityTagManager.Instance.GetTagContainer(inTag, out FAbilityTagContainer tagContainer))
            return TryActivateAbilityByTag(tagContainer);
        return false;
    }
    public bool TryActivateAbilityByTag(FAbilityTagContainer inTag)
    {
        if (abilitiesMap.TryGetValue(inTag, out AbilityBase ability))
            return TryActivateAbility(ability);
        return false;
    }
    public virtual bool TryActivateAbility(AbilityBase inAbility)
    {
        if (CanActivateAbility(inAbility))
        {
            if (inAbility.TryActivateAbility())
            {
                return true;
            }
        }
        return false;
    }
    public virtual bool TryCancelAbilityByTag(FAbilityTagContainer inTagContainer)
    {
        if (abilitiesMap.TryGetValue(inTagContainer, out AbilityBase ability)
            && activityAbilities.Contains(ability))
        {
            ability.CancelAbility();
        }
        return false;
    }
    public virtual bool CanActivateAbility(AbilityBase inAbility)
    {
        return inAbility is object && (inAbility.IsImmediately || (!blockAbilityTagsContainer.HasBlockMatchingTags(inAbility.abilityTags) && CheckSourceTags(inAbility)));
    }

    public virtual void OnActivateAbilitySuccess(AbilityBase inAbility)
    {
        // 移除cancel列表的技能
        CancelAbilityByOther(inAbility);
        // 触发该Tag对应事件
        TriggerEventByAbility(inAbility);
        // 添加至激活列表
        activityAbilities.Add(inAbility);
    }
    public virtual void OnEndAbility(AbilityBase inAbility)
    {
        if (inAbility is object)
        {
            activityAbilities.Remove(inAbility);
            //needRemoveList.Add(inAbility);
        }
    }
    public virtual void RegisterEvent(string inTag, UnityAction<AbilitySystemComponent> inAction)
    {
        RegisterEvent(AbilityTagManager.Instance.GetTagContainer(inTag), inAction);
    }
    public virtual void RegisterEvent(FAbilityTagContainer inTag, UnityAction<AbilitySystemComponent> inAction)
    {
        if (OnAbilityTriggerEventMap.ContainsKey(inTag))
        {
            OnAbilityTriggerEventMap[inTag] += inAction;
        }
        else
        {
            OnAbilityTriggerEventMap.Add(inTag, inAction);
        }
    }
    public virtual void RemoveEvent(string inTag, UnityAction<AbilitySystemComponent> inAction)
    {
        RemoveEvent(AbilityTagManager.Instance.GetTagContainer(inTag), inAction);
    }
    public virtual void RemoveEvent(FAbilityTagContainer inTag, UnityAction<AbilitySystemComponent> inAction)
    {
        if (OnAbilityTriggerEventMap.ContainsKey(inTag))
        {
            OnAbilityTriggerEventMap[inTag] -= inAction;
        }
    }
    public virtual void TriggerEventByAbility(AbilityBase inAbility)
    {
        if (inAbility.activationOwnedTags == null || inAbility.activationOwnedTags.Count == 0 || OnAbilityTriggerEventMap.Count == 0)
            return;

        foreach (FAbilityTagContainer tagContainer in inAbility.activationOwnedTags)
        {
            foreach (FAbilityTagContainer eventContainer in OnAbilityTriggerEventMap.Keys)
            {
                if (tagContainer.HasAll(eventContainer))
                    OnAbilityTriggerEventMap[eventContainer]?.Invoke(null);
            }
        }
    }
    public virtual void CancelAbilityByOther(AbilityBase inAbility)
    {
        if (inAbility.cancelAbilitiesWithTags == null || inAbility.cancelAbilitiesWithTags.Count == 0)
            return;

        foreach (FAbilityTagContainer tagContainer in inAbility.cancelAbilitiesWithTags)
        {
            if (activationAbilityTagsContainer.HasAnyMatchingTags(tagContainer))
            {
                foreach (AbilityBase ability in activityAbilities)
                {
                    if (ability is object && ability.abilityTags.HasAll(tagContainer))
                    {
                        ability.CancelAbility();
                    }
                }
            }
        }
    }
    #endregion

    public void WaitingSelectTargetEvent(ETargetType targetType,KeyCode selectKeyCode, KeyCode unSelectKeyCode, Action<bool, Vector3, AbilitySystemComponent> onSelectTarget)
    {
        StartCoroutine(Cor_WaitingSelectTarget(targetType, selectKeyCode, unSelectKeyCode, onSelectTarget));
    }
    IEnumerator Cor_WaitingSelectTarget(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode, Action<bool, Vector3, AbilitySystemComponent> onSelectTarget)
    {
        if (selectKeyCode == KeyCode.None)
            selectKeyCode = KeyCode.Mouse0;    
        if (unSelectKeyCode == KeyCode.None)
            unSelectKeyCode = KeyCode.Mouse1;

        bool bSelect = false;
        while (!bSelect)
        {
            while (!Input.GetKeyDown(selectKeyCode) && !Input.GetKeyDown(unSelectKeyCode))
                yield return null;

            if (Input.GetKeyDown(selectKeyCode))
            {
                if (targetType == ETargetType.ETT_Target)
                {
                    // 射线检测
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo) && hitInfo.collider != null)
                    {
                        AbilitySystemComponent target = hitInfo.collider.GetComponent<AbilitySystemComponent>();
                        if (target != null)
                        {
                            bSelect = true;
                            onSelectTarget?.Invoke(true, target.transform.position, target);
                        }
                    }
                }
                else
                {
                    // 鼠标位置
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo) && hitInfo.collider != null)
                    {
                        bSelect = true;
                        onSelectTarget?.Invoke(true, hitInfo.point, null);
                    }
                    else
                    {
                        bSelect = true;
                        onSelectTarget?.Invoke(true, transform.forward, null);
                    }
                }
            }
            else
            {
                bSelect = true;
                onSelectTarget?.Invoke(false, Vector3.zero, null);
            }
            yield return null;
        }
    }


    #region Buff
    public bool TryActivateBuffByTag(string inTag,int inLevel = 0,int inStackDelta = 1,bool bForceActivate = false)
    {
        if (AbilityTagManager.Instance.GetTagContainer(inTag, out FAbilityTagContainer abilityTagContainer))
        {
            return TryActivateBuffByTag(abilityTagContainer, inLevel, inStackDelta, bForceActivate);
        }
        return false;
    }
    public bool TryActivateBuffByTag(FAbilityTagContainer inTag, int inLevel = 0, int inStackDelta = 1, bool bForceActivate = false)
    {
        if (buffMap.TryGetValue(inTag, out AbilityBuff buff))
        {
            return TryActivateBuff(buff, inLevel, inStackDelta);
        }
        else
        {
            AbilityBuff tempBuff = AbilityManager.Instance.CreateAbility(inTag, this) as AbilityBuff;
            return TryActivateBuff(tempBuff, inLevel, inStackDelta, bForceActivate);
        }
    }
    public virtual bool TryActivateBuff(AbilityBuff inBuff, int inLevel = 0, int inStackDelta = 1, bool bForceActivate = false)
    {
        if (inBuff == null || (!bForceActivate && !CanActivateAbility(inBuff))) return false;
        if (buffMap.ContainsKey(inBuff.abilityTags))
        {
            if (activityAbilities.Contains(inBuff))
            {
                inBuff.UpdateLevelAndStack(inLevel, inStackDelta);
                return true;
            }
            else if (inBuff.TryActivateAbility())
            {
                inBuff.Stack = inStackDelta;
                activityAbilities.Add(inBuff);
                return true;
            }
            return false;
        }
        else
        {
            buffMap.Add(inBuff.abilityTags, inBuff);
            return TryActivateBuff(inBuff, inLevel, inStackDelta);
        }
    }
    #endregion

    #region Other Check
    public bool CheckSourceTags(AbilityBase inAbility)
    {
        return !activationAbilityTagsContainer.HasBlockMatchingTags(inAbility.sourceBlockedTags) && activationAbilityTagsContainer.HasAnyMatchingTags(inAbility.sourceRequiredTags);
    }
    public bool CheckTargetTags(AbilityBase inAbility)
    {
        return activationAbilityTagsContainer.HasAnyMatchingTags(inAbility.targetRequiredTags) && !activationAbilityTagsContainer.HasBlockMatchingTags(inAbility.targetBlockedTags);
    }
    public void AddBlockTags(List<FAbilityTagContainer> inTags)
    {
        if (inTags == null) return;
        foreach (FAbilityTagContainer tag in inTags)
        {
            blockAbilityTagsContainer.AddTags(tag);
        }
    }
    public void RemoveBlockTags(List<FAbilityTagContainer> inTags)
    {
        if (inTags == null) return;
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
        if (inTags == null) return;
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
        if (inTags == null) return;
        foreach (FAbilityTagContainer tag in inTags)
        {
            activationAbilityTagsContainer.RemoveTags(tag);
        }
    }
    #endregion
}
