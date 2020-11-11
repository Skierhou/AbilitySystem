using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaitingSelectTarget : CustomYieldInstruction
{
    public ESelectTarget selectTargetStatus;
    public List<AbilitySystemComponent> selectedAbilitySystems;

    protected const KeyCode DEFAULT_SELECTKEY = KeyCode.Mouse0;
    protected const KeyCode DEFAULT_UNSELECTKEY = KeyCode.Mouse1;
    public override bool keepWaiting => selectTargetStatus == ESelectTarget.ST_WaitingSelect;

    IEnumerator WaitingSelect(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode,Ability ability)
    {
        if (selectKeyCode == KeyCode.None)
            selectKeyCode = DEFAULT_SELECTKEY;
        if (unSelectKeyCode == KeyCode.None)
            unSelectKeyCode = DEFAULT_UNSELECTKEY;

        //ability.abilityData.spellRange
        selectedAbilitySystems = new List<AbilitySystemComponent>();
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
                            selectedAbilitySystems.Add(target);
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
                        //SelectRangeTarget();
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

    public WaitingSelectTarget(ETargetType targetType, KeyCode selectKeyCode, KeyCode unSelectKeyCode, AbilitySystemComponent abilitySystem, Ability ability)
    {
        selectTargetStatus = ESelectTarget.ST_WaitingSelect;
        abilitySystem.StartCoroutine(WaitingSelect(targetType, selectKeyCode, unSelectKeyCode, ability));
    }
    protected virtual List<Collider> SelectRangeTarget(Vector3 inLoc, EOverlapType intSelectType, Vector3 inRange, Vector3 inFwd)
    {
        List<Collider> colliders = null;
        switch (intSelectType)
        {
            case EOverlapType.Sphere:
                colliders = Physics.OverlapSphere(inLoc, inRange.x * 0.5f).ToList();
                break;
            case EOverlapType.Box:
                inRange.z *= 0.5f;
                colliders = Physics.OverlapBox(inLoc, inRange).ToList();
                break;
            case EOverlapType.Cylinder:
                colliders = Physics.OverlapCapsule(inLoc, inLoc + inRange.y * Vector3.up, inRange.x * 0.5f).ToList();
                break;
            case EOverlapType.Sector:
                //x = z = radius*2，y:角度
                Collider[] temp = Physics.OverlapSphere(inLoc, inRange.x * 0.5f);
                colliders = new List<Collider>();
                foreach (Collider collider in temp)
                {
                    if (Vector3.Dot(inFwd, (collider.transform.position - inLoc).normalized) > inRange.y * 0.5f * Mathf.Deg2Rad)
                    {
                        colliders.Add(collider);
                    }
                }
                break;
            case EOverlapType.Triangle:
                colliders = Physics.OverlapSphere(inLoc, inRange.x * 0.5f).ToList();
                break;
        }
        return colliders;
    }
}