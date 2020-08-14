using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystemComponent : MonoBehaviour
{
    List<Ability> abilities;
    Dictionary<uint, Ability> abilitiyMaps;

    /// <summary>
    /// 获得能力
    /// </summary>
    public virtual void AcquireAbility(Ability ability) { }
    /// <summary>
    /// 移除能力
    /// </summary>
    public virtual void RemoveAbility(Ability ability) { }

    /// <summary>
    /// 尝试触发一个能力
    /// </summary>
    public virtual void TryActivateAbilityByTag(string abilityTag) { }
    public virtual void TryActivateAbility(Ability ability) { }
}
