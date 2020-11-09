using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPhysicsType
{
    PT_None,
    PT_Falling,
    PT_Grounded,
    PT_Flying,
    PT_Swiming,
    PT_Climb,
};

public class Character : MonoBehaviour
{
    #region Properity
    //Normal
    public EPhysicsType physicsType;

    public AbilitySystemComponent AbilitySystemComponent { get; protected set; }
    public Controller Controller { get; protected set; }
    public MovementComponent MovementComponent { get; protected set; }
    #endregion

    public virtual void OnPossess(Controller controller)
    {
        this.Controller = controller;
        if (MovementComponent != null)
            MovementComponent.OnPossess(controller);
    }
    public virtual void OnUnPossess()
    {
        Controller = null;
        if (MovementComponent != null)
            MovementComponent.OnUnPossess();
    }

    #region Unity Life
    private void Awake()
    {
        AbilitySystemComponent = GetComponent<AbilitySystemComponent>();
        MovementComponent = GetComponent<MovementComponent>();
    }
    #endregion
}