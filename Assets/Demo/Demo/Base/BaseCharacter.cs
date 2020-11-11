using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : Character
{
    AbilitySystemComponent abilitySystem;
    Animator animator;

    public Slider hp_slider;
    public Slider mp_slider;

    private void Start()
    {
        abilitySystem = GetComponent<AbilitySystemComponent>();
        animator = GetComponentInChildren<Animator>();
        abilitySystem.AcquireAbilityByTag(AbilityConsts.Instance.Fire);
        abilitySystem.AcquireAbilityByTag(AbilityConsts.Instance.Ice);
        abilitySystem.AcquireAbilityByTag(AbilityConsts.Instance.Wood);
        abilitySystem.AcquireAbilityByTag(AbilityConsts.Instance.Soil);
        abilitySystem.AcquireAbilityByTag(AbilityConsts.Instance.Gold);

        abilitySystem.AttributeSet.RegisterDataChangedEvent(EAttributeType.AT_Health, OnHealthChanged);
        abilitySystem.AttributeSet.RegisterDataChangedEvent(EAttributeType.AT_Mana, OnManaChanged);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            abilitySystem.TryActivateAbilityByTag(AbilityConsts.Instance.Fire);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            abilitySystem.TryActivateAbilityByTag(AbilityConsts.Instance.Gold);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            abilitySystem.TryActivateAbilityByTag(AbilityConsts.Instance.Ice);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            abilitySystem.TryActivateAbilityByTag(AbilityConsts.Instance.Soil);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            abilitySystem.TryActivateAbilityByTag(AbilityConsts.Instance.Wood);

        if (animator != null)
        {
            animator.SetBool("IsGrounded", MovementComponent.IsGrounded);
            if (Controller.GetInputVelocity().sqrMagnitude > 0.1f && MovementComponent.Velocity.sqrMagnitude > 0.1f)
                animator.SetFloat("Fwd", 1.0f);
            else
                animator.SetFloat("Fwd", 0.0f);
        }
    }

    void OnHealthChanged(float baseValue,float currentValue)
    {
        if(hp_slider != null)
            hp_slider.value = currentValue / baseValue;
    }
    void OnManaChanged(float baseValue, float currentValue)
    {
        if (mp_slider != null)
            mp_slider.value = currentValue / baseValue;
    }
}
