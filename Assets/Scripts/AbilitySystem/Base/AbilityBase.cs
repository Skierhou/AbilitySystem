using System;
using System.Collections.Generic;
using UnityEditor;

public class AbilityBase
{
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
}
