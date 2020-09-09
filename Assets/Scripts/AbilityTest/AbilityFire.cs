using System;
using UnityEngine;

public class AbilityFire : Ability
{
    public Texture2D texture;

    public AbilityFire(FAbilityData data, AbilitySystemComponent abilitySystemComponent) : base(data, abilitySystemComponent)
    {
        this.abilityData = data;
        this.abilitySystem = abilitySystemComponent;

        //if (abilitySystem != null)
        //{
        //    abilitySystem.AcquireAbility(this);
        //}
        data.bIsBlockingOtherAbilities = true;
        if (AbilityTagManager.Instance.GetTagContainer("Ability.Ice", out FAbilityTagContainer tagContainer))
        {
            //cancelAbilitiesWithTags = tagContainer;
            //blockAbilitiesWithTags = tagContainer;
        }
    }

    protected override void OnAbilityInit()
    {
        base.OnAbilityInit();
        Debug.Log("Fire_Init");
    }
    protected override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Debug.Log("Fire_Start");
    }
    protected override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        Debug.Log("Fire_End");
    }
    protected override void OnChannelStart()
    {
        base.OnChannelStart();
        Debug.Log("Fire_ChannelStart");
    }
    protected override void OnChannelEnd()
    {
        base.OnChannelEnd();
        Debug.Log("Fire_ChannelEnd");
    }
}