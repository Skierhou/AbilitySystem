using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESpellTimeType
{
    ST_Start,
    ST_Spell,
    ST_End,
};

public class AbilityTest : Ability
{
    [AbilityConfig]
    public ESpellTimeType spellTimeType;

    protected override void OnAbilityStart()
    {
        base.OnAbilityStart();
        ActivitySelfBuff(ESpellTimeType.ST_Start);
    }
    protected override void OnAbilitySpell()
    {
        base.OnAbilitySpell();
        ActivitySelfBuff(ESpellTimeType.ST_Spell);

    }
    protected override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        ActivitySelfBuff(ESpellTimeType.ST_End);
    }

    protected void ActivitySelfBuff(ESpellTimeType inSpellType)
    {
        //if (inSpellType == spellTimeType)
        //{
        //    foreach (var buff in activity_Self_Buffs)
        //    {
        //        abilitySystem.TryActivateBuffByTag(buff);
        //    }
        //}
    }
}
