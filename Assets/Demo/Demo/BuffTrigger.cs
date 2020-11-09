using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTrigger : MonoBehaviour
{
    public string triggerTag;

    private void OnTriggerEnter(Collider other)
    {
        AbilitySystemComponent abilitySystem = other.GetComponent<AbilitySystemComponent>();
        if (abilitySystem is object)
        {
            abilitySystem.TryActivateBuffByTag(triggerTag);
        }
    }
}
