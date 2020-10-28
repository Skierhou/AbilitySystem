public interface IAbilityManager
{
    bool GetBuffData(FAbilityTagContainer inTag, out FAbilityBuffData outBuffData);
    FAbilityBuffData GetBuffData(FAbilityTagContainer inTag);

    bool GetAbilityData(FAbilityTagContainer inTag, out FAbilityData outData);
    FAbilityData GetAbilityData(FAbilityTagContainer inTag);

    Ability CreateAbility(FAbilityTagContainer inTag);
    AbilityBuff CreateAblilityBuff(FAbilityTagContainer inTag, FAbilityBuffData inData);
}