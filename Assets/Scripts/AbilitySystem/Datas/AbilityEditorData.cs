using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Editor_FAbilityBuffData
{
    public EDurationPolicy durationPolicy;

    public List<float> duration;
    public List<float> interval;
};
[System.Serializable]
public struct Editor_FModifierData
{
    public EAttributeType attributeType;
    public EBuffModifierOption modifierOption;
    public EBuffModifierType modifierType;
    public List<float> attributeMagnitudeList;
};

[System.Serializable]
public struct Editor_FMotionModifierData
{
    /* 优先级 */
    public int priority;
    public List<float> duration;

    public EMotionType moveType;
    public EDirectType direction;
    public List<float> distance;

    public EMotionType rotateType;
    public EDirectType rotateAxis;
    public List<float> rotateAngle;

    public int moveCurve;
    public int rotateCurve;
};

[CreateAssetMenu(fileName = "Assets/Scripts/AbilitySystem/Datas/NewAbilityConfig", menuName = "Ability/Create_AbilityConfig", order = 0)]
[System.Serializable]
public class AbilityEditorData : ScriptableObject
{
    public MonoScript AbilityScript;
    public EAbilityType abilityType;

    public List<string> passiveAbilityListenerTags;
    public List<string> passiveAbilityTriggerTags;

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
    public int maxStack = 1;

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

    public AbilityEditorData Buff_CoolDown;
    public AbilityEditorData Buff_Cost;

    public List<string> Activity_Self_Buffs;
    public List<string> Activity_Target_Buffs;

    public Editor_FAbilityBuffData Buff_Data;
    public List<Editor_FModifierData> Buff_Modifiers;

    public List<Editor_FMotionModifierData> Buff_MotionModifiers;

    public List<UnityEngine.Object> child_UnityDatas;
    /** 暂时只支持子类int/bool/float/string类型数据 */
    public List<int> child_BaseDatas_Int;
    public List<bool> child_BaseDatas_Bool;
    public List<float> child_BaseDatas_Float;
    public List<string> child_BaseDatas_String;

    public static void ReadChildInfo(Type type, AbilityEditorData abilityEditorData, object owner)
    {
        int int_index = 0, float_index = 0, string_index = 0, bool_index = 0, unity_index = 0;
        while (type != typeof(AbilityBase) && type != typeof(UnityEngine.Object) && type != typeof(System.Object))
        {
            FieldInfo[] tFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            if (tFieldInfos.Length > 0)
            {
                for (int i = 0; i < tFieldInfos.Length; i++)
                {
                    if (tFieldInfos[i].GetCustomAttribute(typeof(AbilityConfig)) == null)
                        continue;
                    object obj;
                    if (tFieldInfos[i].FieldType == typeof(int))
                        obj = abilityEditorData.child_BaseDatas_Int[int_index++];
                    else if (tFieldInfos[i].FieldType == typeof(byte))
                        obj = System.Convert.ChangeType(abilityEditorData.child_BaseDatas_Int[int_index++], tFieldInfos[i].FieldType);
                    else if (tFieldInfos[i].FieldType.IsEnum)
                        obj = Enum.GetValues(tFieldInfos[i].FieldType).GetValue(abilityEditorData.child_BaseDatas_Int[int_index++]);
                    else if (tFieldInfos[i].FieldType == typeof(bool))
                        obj = abilityEditorData.child_BaseDatas_Bool[bool_index++];
                    else if (tFieldInfos[i].FieldType == typeof(float) || tFieldInfos[i].FieldType == typeof(double))
                        obj = abilityEditorData.child_BaseDatas_Float[float_index++];
                    else if (tFieldInfos[i].FieldType == typeof(string))
                        obj = abilityEditorData.child_BaseDatas_String[string_index++];
                    else if (tFieldInfos[i].FieldType == typeof(char))
                        obj = abilityEditorData.child_BaseDatas_String[string_index++][0];
                    else
                        obj = unity_index < abilityEditorData.child_UnityDatas.Count ? abilityEditorData.child_UnityDatas[unity_index++] : null;
                    if(obj != null)
                        tFieldInfos[i].SetValue(owner, obj);
                }
            }
            type = type.BaseType;
        }
    }
}