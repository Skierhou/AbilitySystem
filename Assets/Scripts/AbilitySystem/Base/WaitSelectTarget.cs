using System;
using System.Collections;
using UnityEngine;

public class WaitSelectTarget : CustomYieldInstruction
{
    public ESelectTarget selectResult;

    protected const KeyCode DEFAULT_SELECTKEYCODE = KeyCode.Mouse0;
    protected const KeyCode DEFAULT_UNSELECTKEYCODE = KeyCode.Mouse1;

    public override bool keepWaiting
    {
        get { return selectResult == ESelectTarget.ST_WaitingSelect; }
    }

    IEnumerator Cor_Waiting(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode, AbilitySystemComponent abilitySystem, AbilityBase ability)
    {
        if (selectKeyCode == KeyCode.None)
            selectKeyCode = DEFAULT_SELECTKEYCODE;
        if (unSelectKeyCode == KeyCode.None)
            unSelectKeyCode = DEFAULT_UNSELECTKEYCODE;

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
                        if (target != null && abilitySystem.CheckTargetTags(ability))
                        {
                            selectResult = ESelectTarget.ST_SelectSuccess;
                        }
                        else
                        {
                            selectResult = ESelectTarget.ST_WaitingSelect;
                            //Warning TODO
                        }
                    }
                    else
                    {
                        selectResult = ESelectTarget.ST_WaitingSelect;
                        //Warning TODO
                    }
                }
                else if (targetType == ETargetType.ETT_Ground)
                {
                    // 鼠标位置
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo) && hitInfo.collider != null)
                    {
                        selectResult = ESelectTarget.ST_SelectSuccess;
                    }
                    else
                    {
                        selectResult = ESelectTarget.ST_WaitingSelect;
                        //Warning TODO
                    }
                }
            }
            else
            {
                selectResult = ESelectTarget.ST_SelectFail;
            }
            yield return null;
        }
    }

    public WaitSelectTarget(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode, AbilitySystemComponent abilitySystem, AbilityBase ability)
    {
        selectResult = ESelectTarget.ST_WaitingSelect;
        abilitySystem.StartCoroutine(Cor_Waiting(targetType, selectKeyCode, unSelectKeyCode, abilitySystem, ability));
    }
}