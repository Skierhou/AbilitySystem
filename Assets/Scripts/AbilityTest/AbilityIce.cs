using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBuffTest : AbilityBuff { }

public class AbilityIce : Ability
{
    public AbilityIce(FAbilityData data, AbilitySystemComponent abilitySystemComponent) : base(data, abilitySystemComponent)
    {
        this.abilityData = data;
        this.abilitySystem = abilitySystemComponent;

        //if (abilitySystem != null)
        //{
        //    abilitySystem.AcquireAbility(this);
        //}
        abilityData.bIsBlockingOtherAbilities = true;
        abilityData.bIsCancelable = true;
        if (AbilityTagManager.Instance.GetTagContainer("Ability.Ice", out FAbilityTagContainer tagContainer1))
        {
            abilityTags = tagContainer1;
        }
        //if (AbilityTagManager.Instance.GetTagContainer("Ability.Fire", out FAbilityTagContainer tagContainer2))
        //{
        //    blockAbilitiesWithTags = tagContainer2;
        //}
    }

    protected override void OnAbilityInit()
    {
        base.OnAbilityInit();
        Debug.Log("Ice_Init");
    }
    protected override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Debug.Log("Ice_Start");
    }
    protected override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        Debug.Log("Ice_End");
    }
    protected override void OnChannelStart()
    {
        base.OnChannelStart();
        Debug.Log("Ice_ChannelStart");
    }
    protected override void OnChannelEnd()
    {
        base.OnChannelEnd();
        Debug.Log("Ice_ChannelEnd");
    }
}