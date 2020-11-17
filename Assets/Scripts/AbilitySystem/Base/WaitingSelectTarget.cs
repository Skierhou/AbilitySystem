using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaitingSelectTarget : CustomYieldInstruction
{
    public ESelectTarget SelectTargetStatus { get; protected set; }
    public List<AbilitySystemComponent> SelectedAbilitySystems { get; protected set; }
    public AbilitySystemComponent SelectedAbilitySystem { get; protected set; }

    protected const KeyCode DEFAULT_SELECTKEY = KeyCode.Mouse0;
    protected const KeyCode DEFAULT_UNSELECTKEY = KeyCode.Mouse1;
    public override bool keepWaiting => SelectTargetStatus == ESelectTarget.ST_WaitingSelect;

    IEnumerator WaitingSelect(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode, AbilitySystemComponent abilitySystem, Ability ability)
    {
        if (selectKeyCode == KeyCode.None)
            selectKeyCode = DEFAULT_SELECTKEY;
        if (unSelectKeyCode == KeyCode.None)
            unSelectKeyCode = DEFAULT_UNSELECTKEY;

        //ability.abilityData.spellRange
        while (keepWaiting)
        {
            while (!Input.GetKeyDown(selectKeyCode) && !Input.GetKeyDown(unSelectKeyCode))
                yield return null;

            if (Input.GetKeyDown(selectKeyCode))
            {
                if (targetType == ETargetType.ETT_Target)
                {
                    // 射线检测
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo) && hitInfo.collider != null)
                    {
                        AbilitySystemComponent target = hitInfo.collider.GetComponent<AbilitySystemComponent>();
                        if (target != null && target.CheckTargetTags(ability))
                        {
                            SelectTargetStatus = ESelectTarget.ST_SelectSuccess;
                            SelectedAbilitySystem = target;
                        }
                        else
                        {
                            SelectTargetStatus = ESelectTarget.ST_WaitingSelect;
                            //TODO Warning
                        }
                    }
                }
                else
                {
                    // 鼠标位置
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo) && hitInfo.collider != null)
                    {
                        SelectTargetStatus = ESelectTarget.ST_SelectSuccess;
                        SelectedAbilitySystems = MathEx.OverlapComponents<AbilitySystemComponent>(ability.AbilityData.spellOverlapType, ability.AbilityData.spellRange, new FTransformData(abilitySystem.transform));
                    }
                    else
                    {
                        SelectTargetStatus = ESelectTarget.ST_WaitingSelect;
                        //TODO Warning
                    }
                }
            }
            else
            {
                SelectTargetStatus = ESelectTarget.ST_SelectFail;
            }
            yield return null;
        }
    }

    public WaitingSelectTarget(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode, AbilitySystemComponent abilitySystem, Ability ability)
    {
        SelectTargetStatus = ESelectTarget.ST_WaitingSelect;
        abilitySystem.StartCoroutine(WaitingSelect(targetType, selectKeyCode, unSelectKeyCode, abilitySystem, ability));
    }
}