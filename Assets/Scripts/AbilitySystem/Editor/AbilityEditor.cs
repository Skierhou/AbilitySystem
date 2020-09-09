using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Editor_FAbilityBuffData
{
    public EAttributeType attributeType;
    public EBuffModifierOption modifierOption;
    public EBuffModifierType modifierType;
    public List<float> attributeMagnitudeList;

    public EDurationPolicy durationPolicy;

    public float duration;
    public float interval;

    public int level;
    public int stack;
};

[CreateAssetMenu(fileName = "Assets/Scripts", menuName = "CreateEditorConfig", order = 0)]
[System.Serializable]
public class AbilityEditor : ScriptableObject
{
    public MonoScript AbilityScript;
    public EAbilityType abilityType;

    [Header("Is Open Block/Cannelable/Immediate")]
    public bool bImmediately;
    public bool bIsCancelable = true;
    public bool bIsBlockingOtherAbilities = true;

    [Header("Normal")]
    public ETargetType targetType;
    public float castPoint;
    public float totalTime;

    [Header("LevelInfo")]
    public int maxLevel = 1;

    [Header("Channel Skill")]
    public float channelStartTime;
    public float channelInterval;
    public float channelEndTime;

    [Header("OtherTags")]
    public List<string> abilityTags;
    /** Abilities with these tags are cancelled when this ability is executed */
    public List<string> cancelAbilitiesWithTags;
    /** Abilities with these tags are blocked while this ability is active*/
    public List<string> blockAbilitiesWithTags;
    /** Tags to apply to activating owner while this ability is active */
    public List<string> activationOwnedTags;
    /** This ability can only be source if the activating actor/component has all of these tags */
    public List<string> sourceRequiredTags;
    /** This ability is blocked if the source actor/component has any of these tags */
    public List<string> sourceBlockedTags;
    /** This ability can only be target if the activating actor/component has all of these tags */
    public List<string> targetRequiredTags;
    /** This ability is blocked if the target actor/component has any of these tags */
    public List<string> targetBlockedTags;

    public MonoScript Buff_CoolDown;
    public Editor_FAbilityBuffData Buff_CoolDown_Data;
    public MonoScript Buff_Cost;
    public Editor_FAbilityBuffData Buff_Cost_Data;

    public List<string> child_UnityDatas_Keys;
    public List<Object> child_UnityDatas;

    /** 暂时只支持子类int/bool/float/string类型数据 */
    public List<int> child_BaseDatas_Int = new List<int>();
    public List<bool> child_BaseDatas_Bool = new List<bool>();
    public List<float> child_BaseDatas_Float = new List<float>();
    public List<string> child_BaseDatas_String = new List<string>();

    /** 存放的是其他额外的List */
    public List<string> list_BaseDatas_Key;
    public List<int> list_BaseDatas_Index;
    public List<int> list_BaseDatas_Length;
    public List<int> list_BaseDatas_Int;
    public List<int> list_BaseDatas_Float;
    public List<int> list_BaseDatas_Bool;
    public List<int> list_BaseDatas_String;
}