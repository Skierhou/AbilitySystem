using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

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
    EATT_ActivitySelfBuffTags,
    EATT_ActivityTargetBuffTags,
    EATT_PassiveAbilityListenerTags,
    EATT_Other,
};

[CustomEditor(typeof(AbilityEditorData))]
public class Inspector_AbilityEditorData : Editor
{
    EEditor_AbilityTagType tagWidgitSelectType;

    AbilityEditorData ability;
    private SerializedObject obj;

    private SerializedProperty AbilityScript;
    private SerializedProperty abilityType;

    private SerializedProperty channelStartTime;
    private SerializedProperty channelInterval;
    private SerializedProperty channelEndTime;

    private SerializedProperty targetType;
    private SerializedProperty maxLevel;
    private SerializedProperty maxStack;
    private SerializedProperty castPoint;
    private SerializedProperty totalTime;

    private SerializedProperty bImmediately;
    private SerializedProperty bIsCancelable;
    private SerializedProperty bIsBlockingOtherAbilities;

    private bool bUseCoolDown;
    private bool bUseCost;
    private SerializedProperty Buff_CoolDown;
    private SerializedProperty Buff_Cost;
    private SerializedProperty Buff_Modifiers;
    private SerializedProperty Buff_MotionModifiers;

    private bool bUseChildInfos;
    private bool bUseOtherTags;
    // 开关
    private List<bool> bUseLists;

    int string_index = 0, int_index = 0, float_index = 0, bool_index = 0, unity_index;

    bool bIsBuff = false;

    void OnEnable()
    {
        ability = (AbilityEditorData)target;
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
        maxStack = obj.FindProperty("maxStack");

        bImmediately = obj.FindProperty("bImmediately");
        bIsCancelable = obj.FindProperty("bIsCancelable");
        bIsBlockingOtherAbilities = obj.FindProperty("bIsBlockingOtherAbilities");

        Buff_CoolDown = obj.FindProperty("Buff_CoolDown");
        Buff_Cost = obj.FindProperty("Buff_Cost");
        Buff_Modifiers = obj.FindProperty("Buff_Modifiers");
        Buff_MotionModifiers = obj.FindProperty("Buff_MotionModifiers");

        bUseCoolDown = ability.Buff_CoolDown is object;
        bUseCost = ability.Buff_Cost is object;

        bUseChildInfos = (ability.child_UnityDatas is object && ability.child_UnityDatas.Count > 0);
        if (ability.child_UnityDatas == null)
            ability.child_UnityDatas = new List<UnityEngine.Object>();
        if (ability.child_BaseDatas_Int == null)
            ability.child_BaseDatas_Int = new List<int>();
        if (ability.child_BaseDatas_Bool == null)
            ability.child_BaseDatas_Bool = new List<bool>();
        if (ability.child_BaseDatas_Float == null)
            ability.child_BaseDatas_Float = new List<float>();
        if (ability.child_BaseDatas_String == null)
            ability.child_BaseDatas_String = new List<string>();
        if (ability.Buff_Modifiers == null)
            ability.Buff_Modifiers = new List<Editor_FModifierData>();
        if (ability.Buff_MotionModifiers == null)
            ability.Buff_MotionModifiers = new List<Editor_FMotionModifierData>();
    }
    public override void OnInspectorGUI()
    {
        unity_index = 0; string_index = 0; int_index = 0; float_index = 0; bool_index = 0;
        // self ability tags
        EditorGUILayout.PropertyField(AbilityScript);
        MonoScript ms = (MonoScript)AbilityScript.objectReferenceValue;
        if (ms != null && IsExtendsType(ms.GetClass(), typeof(AbilityBase)))
            ability.AbilityScript = ms;

        if (ability.AbilityScript == null || !IsExtendsType(ability.AbilityScript.GetClass(), typeof(AbilityBase)))
        {
            EditorGUILayout.LabelField("Please reference a class what extend AbilityBase！");
            return;
        }
        bIsBuff = IsExtendsType(ability.AbilityScript.GetClass(), typeof(AbilityBuff));

        CreateAbilityTag(ability.abilityTags, EEditor_AbilityTagType.EATT_AbilityTags, "AbilityTags");

        EditorGUI.indentLevel = 0;
        EditorGUILayout.PropertyField(maxLevel);

        if (!bIsBuff)
        {
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
            else
            {
                CreateAbilityTag(ability.passiveAbilityListenerTags, EEditor_AbilityTagType.EATT_PassiveAbilityListenerTags, "PassiveListenerTags");
            }

            // buff
            EditorGUILayout.Space(10);
            EditorGUI.indentLevel = 0;
            if (bUseCoolDown = EditorGUILayout.ToggleLeft("Is Use CoolDown?", bUseCoolDown))
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(Buff_CoolDown);
                ability.Buff_CoolDown = (AbilityEditorData)Buff_CoolDown.objectReferenceValue;
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
                ability.Buff_Cost = (AbilityEditorData)Buff_Cost.objectReferenceValue;
                #region 暂时保留
                //if (ability.Buff_Cost != null)
                //{
                //    CreateBuffDataInfo(ref ability.Buff_Data);
                //    EditorGUILayout.PropertyField(Buff_Cost_Modifiers);
                //    for (int i = 0; i < Buff_Cost_Modifiers.arraySize; i++)
                //    {
                //        while (ability.Buff_Modifiers.Count <= i)
                //            ability.Buff_Modifiers.Add(new Editor_FModifierData());
                //        Editor_FModifierData data = ability.Buff_Modifiers[i];
                //        data.attributeType = (EAttributeType)Buff_Cost_Modifiers.GetArrayElementAtIndex(i).FindPropertyRelative("attributeType").intValue;
                //        data.modifierOption = (EBuffModifierOption)Buff_Cost_Modifiers.GetArrayElementAtIndex(i).FindPropertyRelative("modifierOption").intValue;
                //        data.modifierType = (EBuffModifierType)Buff_Cost_Modifiers.GetArrayElementAtIndex(i).FindPropertyRelative("modifierType").intValue;
                //        SerializedProperty tSP = Buff_Cost_Modifiers.GetArrayElementAtIndex(i).FindPropertyRelative("attributeMagnitudeList");
                //        if (data.attributeMagnitudeList == null)
                //            data.attributeMagnitudeList = new List<float>();
                //        else
                //            data.attributeMagnitudeList.Clear();
                //        for (int j = 0; j < maxLevel.intValue; j++)
                //        {
                //            if (j < tSP.arraySize)
                //                data.attributeMagnitudeList.Add(tSP.GetArrayElementAtIndex(j).floatValue);
                //            else
                //                data.attributeMagnitudeList.Add(0);
                //        }
                //        tSP.arraySize = maxLevel.intValue;
                //        ability.Buff_Modifiers[i] = data;
                //    }
                //}
                #endregion
            }
            else
            {
                ability.Buff_Cost = null;
            }
            //EditorGUILayout.PropertyField(Activity_Self_Buffs);
            //ability.Activity_Self_Buffs = new List<AbilityEditorData>();
            //for (int i = 0; i < Activity_Self_Buffs.arraySize; i++)
            //{
            //    ability.Activity_Self_Buffs.Add(Activity_Self_Buffs.GetArrayElementAtIndex(i).objectReferenceValue as AbilityEditorData);
            //}
            //EditorGUILayout.PropertyField(Activity_Target_Buffs);
            //ability.Activity_Target_Buffs = new List<AbilityEditorData>();
            //for (int i = 0; i < Activity_Target_Buffs.arraySize; i++)
            //{
            //    ability.Activity_Target_Buffs.Add(Activity_Target_Buffs.GetArrayElementAtIndex(i).objectReferenceValue as AbilityEditorData);
            //}
            //CreateAbilityTag(ability.Activity_Self_Buffs, EEditor_AbilityTagType.EATT_ActivitySelfBuffTags, "ActivitySelfTags");
            //CreateAbilityTag(ability.Activity_Target_Buffs, EEditor_AbilityTagType.EATT_ActivityTargetBuffTags, "ActivityTargetTags");
        }
        else
        {
            EditorGUILayout.PropertyField(maxStack);
            EditorGUILayout.LabelField("Buff Data");
            ++EditorGUI.indentLevel;
            CreateBuffDataInfo(ref ability.Buff_Data);
            --EditorGUI.indentLevel;

            EditorGUILayout.PropertyField(Buff_Modifiers,new GUIContent("Modifiers"));
            while (ability.Buff_Modifiers.Count <= Buff_Modifiers.arraySize)
                ability.Buff_Modifiers.Add(new Editor_FModifierData());
            while (ability.Buff_Modifiers.Count > Buff_Modifiers.arraySize)
                ability.Buff_Modifiers.RemoveAt(ability.Buff_Modifiers.Count - 1);

            for (int i = 0; i < Buff_Modifiers.arraySize; i++)
            {
                Editor_FModifierData data = ability.Buff_Modifiers[i];
                data.attributeType = (EAttributeType)Buff_Modifiers.GetArrayElementAtIndex(i).FindPropertyRelative("attributeType").intValue;
                data.modifierOption = (EBuffModifierOption)Buff_Modifiers.GetArrayElementAtIndex(i).FindPropertyRelative("modifierOption").intValue;
                data.modifierType = (EBuffModifierType)Buff_Modifiers.GetArrayElementAtIndex(i).FindPropertyRelative("modifierType").intValue;
                SetList("attributeMagnitudeList", i, Buff_Modifiers, ref data.attributeMagnitudeList);
                ability.Buff_Modifiers[i] = data;
            }

            EditorGUILayout.PropertyField(Buff_MotionModifiers, new GUIContent("MotionModifiers"));
            while (ability.Buff_MotionModifiers.Count <= Buff_MotionModifiers.arraySize)
                ability.Buff_MotionModifiers.Add(new Editor_FMotionModifierData());
            while (ability.Buff_MotionModifiers.Count > Buff_MotionModifiers.arraySize)
                ability.Buff_MotionModifiers.RemoveAt(ability.Buff_MotionModifiers.Count - 1);
            for (int i = 0; i < Buff_MotionModifiers.arraySize; i++)
            {
                Editor_FMotionModifierData data = ability.Buff_MotionModifiers[i];
                data.priority = Buff_MotionModifiers.GetArrayElementAtIndex(i).FindPropertyRelative("priority").intValue;
                data.moveType = (EMotionType)Buff_MotionModifiers.GetArrayElementAtIndex(i).FindPropertyRelative("moveType").intValue;
                data.direction = (EDirectType)Buff_MotionModifiers.GetArrayElementAtIndex(i).FindPropertyRelative("direction").intValue;
                data.rotateType = (EMotionType)Buff_MotionModifiers.GetArrayElementAtIndex(i).FindPropertyRelative("rotateType").intValue;
                data.rotateAxis = (EDirectType)Buff_MotionModifiers.GetArrayElementAtIndex(i).FindPropertyRelative("rotateAxis").intValue;
                data.moveCurve = Buff_MotionModifiers.GetArrayElementAtIndex(i).FindPropertyRelative("moveCurve").intValue;
                data.rotateCurve = Buff_MotionModifiers.GetArrayElementAtIndex(i).FindPropertyRelative("rotateCurve").intValue;
                SetList("duration", i, Buff_MotionModifiers, ref data.duration);
                SetList("distance", i, Buff_MotionModifiers, ref data.distance);
                SetList("rotateAngle", i, Buff_MotionModifiers, ref data.rotateAngle);
                ability.Buff_MotionModifiers[i] = data;
            }
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
        ability.maxStack = maxStack.intValue;
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
        if (bUseChildInfos = EditorGUILayout.Foldout(bUseChildInfos, "Child Infos :"))
        {
            EditorGUI.indentLevel = 1;
            CreateScriptInfos(ability.AbilityScript.GetClass());
        }
        //Unity .Asset保存有bug 需要自己主动保存，而且需要删除原来的创建新的 不然会丢失数据
        if (GUILayout.Button("保存", GUILayout.MaxWidth(100)))
        {
            AbilityEditorData data = CreateInstance<AbilityEditorData>();
            FieldInfo[] fieldInfos = data.GetType().GetFields();
            foreach (FieldInfo info in fieldInfos)
            {
                info.SetValue(data, info.GetValue(ability));
            }
            string path1 = "Assets/Resources/Ability/" + ability.name + ".asset";
            string path2 = "Assets/Resources/Ability/" + ability.name + "_.asset";

            //AssetDatabase.DeleteAsset(path);
            //AssetDatabase.CreateAsset(data, path);
            //移动资源这种方式保存， 删除再次创建资源引用会丢失
            AssetDatabase.MoveAsset(path1, path2);
            AssetDatabase.MoveAsset(path2, path1);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void OnAbilityTagsChange(List<string> list)
    {
        switch (tagWidgitSelectType)
        {
            case EEditor_AbilityTagType.EATT_AbilityTags:
                if (list.Count > 0)
                    ability.abilityTags = new List<string>() { list[list.Count - 1] };
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
            case EEditor_AbilityTagType.EATT_ActivitySelfBuffTags:
                ability.Activity_Self_Buffs = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_ActivityTargetBuffTags:
                ability.Activity_Target_Buffs = new List<string>(list);
                break;
            case EEditor_AbilityTagType.EATT_PassiveAbilityListenerTags:
                ability.passiveAbilityListenerTags = new List<string>(list);
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
        if (GUILayout.Button("+", GUILayout.MaxWidth(80)))
        {
            tagWidgitSelectType = tagType;
            Window_AbilityTagEditor.Init(list, OnAbilityTagsChange);
        }
        if (GUILayout.Button("Clear", GUILayout.MaxWidth(40)))
        {
            list.Clear();
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
    void CreateBuffDataInfo(ref Editor_FAbilityBuffData data)
    {
        data.durationPolicy = (EDurationPolicy)CreateBaseData(data.durationPolicy.GetType(), "DurationPolicy", data.durationPolicy);
        if (data.durationPolicy.HasFlag(EDurationPolicy.EDP_HasDuration))
        {
            ++EditorGUI.indentLevel;

            data.duration = CreateList("Duration", ability.maxLevel, ref data.duration, 0);
            --EditorGUI.indentLevel;
        }
        if (data.durationPolicy == EDurationPolicy.EDP_HasDuration 
            || data.durationPolicy == EDurationPolicy.EDP_Infinite)
        {
            ++EditorGUI.indentLevel;
            data.interval = CreateList("Interval", ability.maxLevel, ref data.interval, 1);
            --EditorGUI.indentLevel;
        }
    }

    void CreateScriptInfos(Type type)
    {
        while (type != typeof(AbilityBase) && type != typeof(UnityEngine.Object) && type != typeof(System.Object))
        {
            FieldInfo[] tFieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            if (tFieldInfos.Length > 0)
            {
                bool bTitle = false;
                for (int i = 0; i < tFieldInfos.Length; i++)
                {
                    if (tFieldInfos[i].GetCustomAttribute(typeof(AbilityConfig)) == null)
                        continue;
                    if (!bTitle)
                    {
                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.LabelField(type + " :");
                        EditorGUI.indentLevel = 2;
                        bTitle = true;
                    }
                    //if (tFieldInfos[i].FieldType == typeof(List<FAbilityTagContainer>))
                    //{
                    //    CreateAbilityTag(ability.child_BaseDatas_String,string_index,,EEditor_AbilityTagType.EATT_Other, tFieldInfos[i].Name);
                    //}
                    if (IsBaseData(tFieldInfos[i].FieldType))
                    {
                        CreateBaseData(tFieldInfos[i].FieldType, tFieldInfos[i].Name);
                    }
                    else
                    {
                        if (ability.child_UnityDatas.Count <= unity_index)
                            ability.child_UnityDatas.Add(null);
                        ability.child_UnityDatas[unity_index] = EditorGUILayout.ObjectField(tFieldInfos[i].Name, ability.child_UnityDatas[unity_index], tFieldInfos[i].FieldType, true);
                        ++unity_index;
                    }
                }
            }
            type = type.BaseType;
        }
    }
    bool IsBaseData(Type type)
    {
        return type.IsEnum || type.IsValueType || type.IsArray || type == typeof(object) || type == typeof(string);
    }
    /// <summary>
    /// 类型1是否继承自类型2
    /// </summary>
    bool IsExtendsType(Type type,Type targetType)
    {
        while (!IsBaseData(type))
        {
            if (type == targetType)
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }
    void CreateBaseData(Type type, string name)
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
            //CreateList(type, name);
        }
        else
        {
            Debug.LogError("Type : " + type + " is not distinguish !");
        }
    }
    object CreateBaseData(Type type, string name, object defaultValue)
    {
        if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
        {
            int res = (int)defaultValue;
            res = EditorGUILayout.IntField(name, res);
            return res;
        }
        else if (type == typeof(byte))
        {
            int res = (int)defaultValue;
            res = EditorGUILayout.IntField(name, res);
            res = Mathf.Clamp(res, 0, 1024);
            return res;
        }
        else if (type == typeof(bool))
        {
            bool res = (bool)defaultValue;
            res = EditorGUILayout.ToggleLeft(name, res);
            return res;
        }
        else if (type == typeof(char))
        {
            string res = (string)defaultValue;
            res = EditorGUILayout.TextField(name, res);
            if (res.Length > 1)
                res = res[0].ToString();
            return res;
        }
        else if (type == typeof(string))
        {
            string res = (string)defaultValue;
            res = EditorGUILayout.TextField(name, res);
            return res;
        }
        else if (type == typeof(double) || type == typeof(float))
        {
            float res = (float)defaultValue;
            res = EditorGUILayout.FloatField(name, res);
            return res;
        }
        else if (type.IsEnum)
        {
            int res = (int)defaultValue;
            Array array = type.GetEnumValues();
            string[] strs = new string[array.Length];
            int[] ints = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                ints[i] = i;
                strs[i] = array.GetValue(i).ToString();
            }
            res = EditorGUILayout.IntPopup(name, res, strs, ints);
            return res;
        }
        return null;
    }

    List<T> CreateList<T>(string name,int length,ref List<T> refList,int toggleIndex)
    {
        if(refList == null)
            refList = new List<T>();

        if (bUseLists == null)
            bUseLists = new List<bool>();
        while (bUseLists.Count <= toggleIndex)
            bUseLists.Add(true);

        if (bUseLists[toggleIndex] = EditorGUILayout.Foldout(bUseLists[toggleIndex], name))
        {
            ++EditorGUI.indentLevel;
            EditorGUILayout.BeginVertical();
            while (refList.Count > length)
            {
                refList.RemoveAt(refList.Count - 1);
            }
            while (refList.Count < length)
            {
                refList.Add((T)System.Activator.CreateInstance(typeof(T)));
            }
            CreateBaseData(typeof(int), "Size", refList.Count);
            for (int i = 0; i < length; i++)
            {
                refList[i] = (T)CreateBaseData(typeof(T), "Element" + i, refList[i]);
            }
            EditorGUILayout.EndVertical();
            --EditorGUI.indentLevel;
        }
        return refList;
    }

    void SetList(string name, int index, SerializedProperty sp, ref List<float> list)
    {
        SerializedProperty tSP = sp.GetArrayElementAtIndex(index).FindPropertyRelative(name);
        if (list == null)
            list = new List<float>();
        else
            list.Clear();
        for (int j = 0; j < maxLevel.intValue; j++)
        {
            if (j < tSP.arraySize)
                list.Add(tSP.GetArrayElementAtIndex(j).floatValue);
            else
                list.Add(default);
        }
        tSP.arraySize = maxLevel.intValue;
    }
}