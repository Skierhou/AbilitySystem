using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    public Character Character { get; protected set; }

    public float _moveSpeed = 10;

    public virtual void Possess(Character character)
    {
        Character = character;
        Character.OnPossess(this);
    }
    public virtual void UnPossess()
    {
        Character.OnUnPossess();
    }

    public virtual Vector3 GetInputVelocity()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * _moveSpeed;
    }
    public virtual Vector3 GetInputAngularVelocity()
    {
        return Vector3.zero;
        //return new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0).normalized * _moveSpeed;
    }
}
