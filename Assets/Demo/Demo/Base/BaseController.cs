using UnityEngine;

class BaseController:Controller
{
    public override Vector3 GetInputVelocity()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * _moveSpeed;
    }
    public override Vector3 GetInputAngularVelocity()
    {
        return Vector3.zero;
    }
}