using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingSelectTarget : CustomYieldInstruction
{
    public ESelectTarget selectTargetStatus;

    protected const KeyCode DEFAULT_SELECTKEY = KeyCode.Mouse0;
    protected const KeyCode DEFAULT_UNSELECTKEY = KeyCode.Mouse1;
    public override bool keepWaiting => selectTargetStatus == ESelectTarget.ST_WaitingSelect;

    IEnumerator WaitingSelect(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode,AbilityBase ability)
    {
        if (selectKeyCode == KeyCode.None)
            selectKeyCode = DEFAULT_SELECTKEY;
        if (unSelectKeyCode == KeyCode.None)
            unSelectKeyCode = DEFAULT_UNSELECTKEY;

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
                            selectTargetStatus = ESelectTarget.ST_SelectSuccess;
                        }
                        else
                        {
                            selectTargetStatus = ESelectTarget.ST_WaitingSelect;
                            //TODO Warning
                        }
                    }
                }
                else
                {
                    // 鼠标位置
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo) && hitInfo.collider != null)
                    {
                        selectTargetStatus = ESelectTarget.ST_SelectSuccess;
                    }
                    else
                    {
                        selectTargetStatus = ESelectTarget.ST_WaitingSelect;
                        //TODO Warning
                    }
                }
            }
            else
            {
                selectTargetStatus = ESelectTarget.ST_SelectFail;
            }
            yield return null;
        }
    }

    public WaitingSelectTarget(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode, AbilitySystemComponent abilitySystem, AbilityBase ability)
    {
        selectTargetStatus = ESelectTarget.ST_WaitingSelect;
        abilitySystem.StartCoroutine(WaitingSelect(targetType, selectKeyCode, unSelectKeyCode, ability));
    }
}