using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework.Interfaces;
using System.Reflection;
using System;
using UnityEditor.UIElements;

public enum EEditor_AbilityTagType
{
    EATT_AbilityTags,
    EATT_CancelAbilitiesWithTags,
    EATT_BlockAbilitiesWithTags,
    EATT_ActivationOwnedTags,
    EATT_SourceRequiredTags,
    EATT_SourceBlockedTags,
    EATT_TargetRequiredTags,
    EATT_TargetBlockedTags,
};

[CustomEditor(typeof(AbilityEditor))]
public class Inspector_AbilityEditor : Editor
{
    EEditor_AbilityTagType tagWidgitSelectType;

    AbilityEditor ability;
    private SerializedObject obj;

    private SerializedProperty AbilityScript;
    private SerializedProperty abilityType;

    private SerializedProperty channelStartTime;
    private SerializedProperty channelInterval;
    private SerializedProperty channelEndTime;

    private SerializedProperty targetType;
    private SerializedProperty maxLevel;
    private SerializedProperty castPoint;
    private SerializedProperty totalTime;

    private SerializedProperty bImmediately;
    private SerializedProperty bIsCancelable;
    private SerializedProperty bIsBlockingOtherAbilities;

    private bool bUseCoolDown;
    private bool bUseCost;
    private SerializedProperty Buff_CoolDown;
    private SerializedProperty Buff_Cost;

    private bool bUseChildInfos;
    private bool bUseOtherTags;

    int string_index = 0, int_index = 0, float_index = 0, bool_index = 0;

    void OnEnable()
    {
        ability = (AbilityEditor)target;
        obj = new SerializedObject(target);
        AbilityScript = obj.FindProperty("AbilityScript");
        abilityType = obj.FindProperty("abilityType");
        channelStartTime = obj.FindProperty("channelStartTime");
        channelInterval = obj.FindProperty("channelInterval");
        channelEndTime = obj.FindProperty("channelEndTime");

        targetType = obj.FindProperty("targetType");
        castPoint = obj.FindProperty("castPoint");
        totalTime = obj.FindProperty("totalTime");
        maxLevel = obj.FindProperty("maxLevel");

        bImmediately = obj.FindProperty("bImmediately");
        bIsCancelable = obj.FindProperty("bIsCancelable");
        bIsBlockingOtherAbilities = obj.FindProperty("bIsBlockingOtherAbilities");

        Buff_CoolDown = obj.FindProperty("Buff_CoolDown");
        Buff_Cost = obj.FindProperty("Buff_Cost");

        bUseCoolDown = ability.Buff_CoolDown is object;
        bUseCost = ability.Buff_Cost is object;
        //bUseChildInfos = (ability.child_UnityDataMaps is object && ability.child_UnityDataMaps.Count > 0) || (ability.child_BaseDataMaps is object && ability.child_BaseDataMaps.Count > 0);

        //if (ability.child_UnityDataMaps == null)
        //    ability.child_UnityDataMaps = new Dictionary<string, UnityEngine.Object>();
        //if (ability.child_BaseDataMaps == null)
        //    ability.child_BaseDataMaps = new Dictionary<string, object>();

        bUseChildInfos = (ability.child_UnityDatas is object && ability.child_UnityDatas.Count > 0);
        if (ability.child_UnityDatas == null)
            ability.child_UnityDatas = new List<UnityEngine.Object>();
    }
    public override void OnInspectorGUI()
    {
        // self ability tags
        EditorGUILayout.PropertyField(AbilityScript);
        CreateAbilityTag(ability.abilityTags, EEditor_AbilityTagType.EATT_AbilityTags, "AbilityTags");

        // ability type
        EditorGUILayout.Space(10);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.PropertyField(abilityType);
        if (abilityType.intValue == (int)EAbilityType.EAT_ChannelAbility)
        {
            EditorGUILayout.PropertyField(channelStartTime);
            EditorGUILayout.PropertyField(channelInterval);
            EditorGUILayout.PropertyField(channelEndTime);
        }
        if (abilityType.intValue != (int)EAbilityType.EAT_PassiveAblity)
        {
            EditorGUILayout.PropertyField(targetType);
            EditorGUILayout.PropertyField(castPoint);
            EditorGUILayout.PropertyField(totalTime);
        }
        EditorGUILayout.PropertyField(maxLevel);

        // buff
        EditorGUILayout.Space(10);
        EditorGUI.indentLevel = 0;
        if (bUseCoolDown = EditorGUILayout.ToggleLeft("Is Use CoolDown?", bUseCoolDown))
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(Buff_CoolDown);
            ability.Buff_CoolDown = (MonoScript)Buff_CoolDown.objectReferenceValue;
            if (ability.Buff_CoolDown != null)
            {
                //CreateScriptInfos(ability.Buff_CoolDown.GetClass());
                CreateScriptInfos(ability.Buff_CoolDown_Data.GetType());
            }
            //CreateScriptInfos(ability.Buff_CoolDown);
            //EditorGUILayout.PropertyField(obj.FindProperty("Buff_CoolDown_Data"));
        }
        else
        {
            ability.Buff_CoolDown = null;
        }
        EditorGUI.indentLevel = 0;
        if (bUseCost = EditorGUILayout.ToggleLeft("Is Use Cost?", bUseCost))
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(Buff_Cost);
            ability.Buff_Cost = (MonoScript)Buff_Cost.objectReferenceValue;
            if(ability.Buff_Cost != null)
                CreateScriptInfos(ability.Buff_Cost_Data.GetType());
            //CreateScriptInfos(ability.Buff_Cost);
            //EditorGUILayout.PropertyField(obj.FindProperty("Buff_Cost_Data"));
        }
        else
        {
            ability.Buff_Cost = null;
        }

        // other
        EditorGUI.indentLevel = 0;
        EditorGUILayout.PropertyField(bImmediately);
        EditorGUILayout.PropertyField(bIsCancelable);
        EditorGUILayout.PropertyField(bIsBlockingOtherAbilities);

        ability.abilityType = (EAbilityType)abilityType.intValue;
        ability.channelStartTime = channelStartTime.floatValue;
        ability.channelInterval = channelInterval.floatValue;
        ability.channelEndTime = channelEndTime.floatValue;
        ability.targetType = (ETargetType)targetType.intValue;
        ability.castPoint = castPoint.floatValue;
        ability.totalTime = totalTime.floatValue;
        ability.maxLevel = maxLevel.intValue;
        ability.bImmediately = bImmediately.boolValue;
        ability.bIsCancelable = bIsCancelable.boolValue;
        ability.bIsBlockingOtherAbilities = bIsBlockingOtherAbilities.boolValue;
        ability.AbilityScript = (MonoScript)AbilityScript.objectReferenceValue;

        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space(10);
        if (bUseOtherTags = EditorGUILayout.Foldout(bUseOtherTags, "Other Tags :"))
        {
            CreateAbilityTag(ability.cancelAbilitiesWithTags, EEditor_AbilityTagType.EATT_CancelAbilitiesWithTags, "CancelAbilitiesWithTags");
            CreateAbilityTag(ability.blockAbilitiesWithTags, EEditor_AbilityTagType.EATT_BlockAbilitiesWithTags, "BlockAbilitiesWithTags"); ;
            CreateAbilityTag(ability.activationOwnedTags, EEditor_AbilityTagType.EATT_ActivationOwnedTags, "ActivationOwnedTags");
            CreateAbilityTag(ability.sourceRequiredTags, EEditor_AbilityTagType.EATT_SourceRequiredTags, "SourceRequiredTags");
            CreateAbilityTag(ability.sourceBlockedTags, EEditor_AbilityTagType.EATT_SourceBlockedTags, "SourceBlockedTags");
            CreateAbilityTag(ability.targetRequiredTags, EEditor_AbilityTagType.EATT_TargetRequiredTags, "TargetRequiredTags");
            CreateAbilityTag(ability.targetBlockedTags, EEditor_AbilityTagType.EATT_TargetBlockedTags, "TargetBlockedTags");
        }

        EditorGUILayout.Space(10);
        EditorGUI.indentLevel = 0;
        if (bUseChildInfos = EditorGUILayout.Foldout( bUseChildInfos , "Child Infos :"))
        {
            EditorGUI.indentLevel = 1;
            CreateScriptInfos(ability.AbilityScript.GetClass());
        }
    }

    void OnAbilityTagsChange(List<string> list)
    {
        switch (tagWidgitSelectType)
        {
            case EEditor_AbilityTagType.EATT_AbilityTags:
                ability.abilityTags = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_CancelAbilitiesWithTags:
                ability.cancelAbilitiesWithTags = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_BlockAbilitiesWithTags:
                ability.blockAbilitiesWithTags = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_ActivationOwnedTags:
                ability.activationOwnedTags = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_SourceRequiredTags:
                ability.sourceRequiredTags = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_SourceBlockedTags:
                ability.sourceBlockedTags = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_TargetRequiredTags:
                ability.targetRequiredTags = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_TargetBlockedTags:
                ability.targetBlockedTags = new List<string>(list);
                break;
            default:
                break;
        }
    }
    void CreateAbilityTag(List<string> list, EEditor_AbilityTagType tagType, string name)
    {
        EditorGUI.indentLevel = 0;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name + " : ");
        if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
        {
            tagWidgitSelectType = tagType;
            Window_AbilityTagEditor.Init(list, OnAbilityTagsChange);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel = 1;
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.LabelField(list[i]);
            }
        }
        EditorGUILayout.Space(5);
    }

    void CreateScriptInfos(Type type)
    {
        while (type != typeof(AbilityBase) && type != typeof(Ability) && type != typeof(UnityEngine.Object) && type != typeof(System.Object))
        {
            FieldInfo[] tFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            if (tFieldInfos.Length > 0)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField(type + " :");
                EditorGUI.indentLevel = 2;

                int unity_index = 0;
                string_index = 0; int_index = 0; float_index = 0; bool_index = 0;
                for (int i = 0; i < tFieldInfos.Length; i++)
                {
                    if (IsBaseData(tFieldInfos[i].FieldType))
                    {
                        CheckBaseData(tFieldInfos[i].FieldType, tFieldInfos[i].Name);
                    }
                    else
                    {
                        if (ability.child_UnityDatas.Count <= unity_index)
                            ability.child_UnityDatas.Add(null);
                        ability.child_UnityDatas[unity_index] = EditorGUILayout.ObjectField(tFieldInfos[i].Name, ability.child_UnityDatas[unity_index], tFieldInfos[i].FieldType, true);
                        ++unity_index;
                    }
                }
                //foreach (FieldInfo info in tFieldInfos)
                //{
                //    if (IsBaseData(info.FieldType))
                //    {
                //        CheckBaseData(info.FieldType, info.Name);
                //    }
                //    else
                //    {
                //        if (!ability.child_UnityDataMaps.ContainsKey(info.Name))
                //        {
                //            ability.child_UnityDataMaps.Add(info.Name, null);
                //        }
                //        ability.child_UnityDataMaps[info.Name] = EditorGUILayout.ObjectField(info.Name, ability.child_UnityDataMaps[info.Name], info.FieldType, true);
                //    }
                //}
            }
            type = type.BaseType;
        }
    }
    bool IsBaseData(Type type)
    {
        return type.IsEnum || type.IsValueType || type.IsArray || type == typeof(object) || type.BaseType == typeof(object);
    }
    void CheckBaseData(Type type, string name)
    {
        if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
        {
            if (ability.child_BaseDatas_Int.Count <= int_index)
                ability.child_BaseDatas_Int.Add(0);
            ability.child_BaseDatas_Int[int_index] = EditorGUILayout.IntField(name, ability.child_BaseDatas_Int[int_index]);
            ++int_index;
        }
        else if (type == typeof(byte))
        {
            if (ability.child_BaseDatas_Int.Count <= int_index)
                ability.child_BaseDatas_Int.Add(0);
            ability.child_BaseDatas_Int[int_index] = EditorGUILayout.IntField(name, ability.child_BaseDatas_Int[int_index]);
            ability.child_BaseDatas_Int[int_index] = Mathf.Clamp(ability.child_BaseDatas_Int[int_index], 0, 1024);
            ++int_index;
        }
        else if (type == typeof(bool))
        {
            if (ability.child_BaseDatas_Bool.Count <= bool_index)
                ability.child_BaseDatas_Bool.Add(true);
            ability.child_BaseDatas_Bool[bool_index] = EditorGUILayout.ToggleLeft(name, ability.child_BaseDatas_Bool[bool_index]);
            ++bool_index;
        }
        else if (type == typeof(char))
        {
            if (ability.child_BaseDatas_String.Count <= string_index)
                ability.child_BaseDatas_String.Add("");
            ability.child_BaseDatas_String[string_index] = EditorGUILayout.TextField(name, ability.child_BaseDatas_String[string_index].ToString());
            if (ability.child_BaseDatas_String[string_index].ToString().Length > 1)
                ability.child_BaseDatas_String[string_index] = ability.child_BaseDatas_String[string_index][0].ToString();
            ++string_index;
        }
        else if (type == typeof(string))
        {
            if (ability.child_BaseDatas_String.Count <= string_index)
                ability.child_BaseDatas_String.Add("");
            ability.child_BaseDatas_String[string_index] = EditorGUILayout.TextField(name, ability.child_BaseDatas_String[string_index]);
            ++string_index;
        }
        else if (type == typeof(double) || type == typeof(float))
        {
            if (ability.child_BaseDatas_Float.Count <= float_index)
                ability.child_BaseDatas_Float.Add(0.0f);
            ability.child_BaseDatas_Float[float_index] = EditorGUILayout.FloatField(name, ability.child_BaseDatas_Float[float_index]);
            ++float_index;
        }
        else if (type.IsEnum)
        {
            Array array = type.GetEnumValues();
            string[] strs = new string[array.Length];
            int[] ints = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                ints[i] = i;
                strs[i] = array.GetValue(i).ToString();
            }
            if (ability.child_BaseDatas_Int.Count <= int_index)
                ability.child_BaseDatas_Int.Add(0);
            ability.child_BaseDatas_Int[int_index] = EditorGUILayout.IntPopup(name, ability.child_BaseDatas_Int[int_index], strs, ints);
            ++int_index;
        }
        else if (type.IsArray || type.IsGenericType || type.IsConstructedGenericType)
        {
            CreateList(type, name);
        }
        else
        {
            Debug.LogError("Type : " + type + " is not distinguish !");
        }
    }
    //bool CheckIsList(Type inType)
    //{
    //    return inType.ToString().Contains("System.Collections.Generic.List");
    //}
    void CreateList(Type inType,string inName)
    {
        Type type = GetListType(inType.ToString());
        if (type != null)
        {
            EditorGUI.indentLevel++;
            Debug.LogWarning(type);
        }
    }
    private Type GetListType(string inTypeStr)
    {
        int startIndex = inTypeStr.LastIndexOf('[') + 1;
        string type = inTypeStr.Substring(startIndex, inTypeStr.LastIndexOf(']') - startIndex);
        return Type.GetType(type, false, true);
    }

    //void CheckBaseData(Type type,string name)
    //{
    //    if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
    //    {
    //        if (!ability.child_BaseDataMaps.ContainsKey(name))
    //            ability.child_BaseDataMaps.Add(name, 0);
    //        ability.child_BaseDataMaps[name] = EditorGUILayout.IntField(name, (int)ability.child_BaseDataMaps[name]);
    //    }
    //    else if (type == typeof(bool))
    //    {
    //        if (!ability.child_BaseDataMaps.ContainsKey(name))
    //            ability.child_BaseDataMaps.Add(name, true);
    //        ability.child_BaseDataMaps[name] = EditorGUILayout.ToggleLeft(name, (bool)ability.child_BaseDataMaps[name]);
    //    }
    //    else if (type == typeof(byte))
    //    {
    //        if (!ability.child_BaseDataMaps.ContainsKey(name))
    //            ability.child_BaseDataMaps.Add(name, 0);
    //        ability.child_BaseDataMaps[name] = EditorGUILayout.IntSlider(name, (int)ability.child_BaseDataMaps[name], 0, 1024);
    //    }
    //    else if (type == typeof(char))
    //    {
    //        if (!ability.child_BaseDataMaps.ContainsKey(name))
    //            ability.child_BaseDataMaps.Add(name, "");
    //        ability.child_BaseDataMaps[name] = EditorGUILayout.TextField(name, ability.child_BaseDataMaps[name].ToString());
    //        if (ability.child_BaseDataMaps[name].ToString().Length > 1)
    //            ability.child_BaseDataMaps[name] = ability.child_BaseDataMaps[name].ToString()[0];
    //    }
    //    else if (type == typeof(string))
    //    {
    //        if (!ability.child_BaseDataMaps.ContainsKey(name))
    //            ability.child_BaseDataMaps.Add(name, "");
    //        ability.child_BaseDataMaps[name] = EditorGUILayout.TextField(name, (string)ability.child_BaseDataMaps[name]);
    //    }
    //    else if (type == typeof(double) || type == typeof(float))
    //    {
    //        if (!ability.child_BaseDataMaps.ContainsKey(name))
    //            ability.child_BaseDataMaps.Add(name, 0.0f);
    //        ability.child_BaseDataMaps[name] = EditorGUILayout.FloatField(name, (float)ability.child_BaseDataMaps[name]);
    //    }
    //    else
    //    {
    //        Debug.LogError("Type : " + type + " is not distinguish !");
    //    }
    //}

}