using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceProjectile : Projectile
{
    [SerializeField] protected int MaxBounceCount;
    [SerializeField] protected float DecreasingRate;
    [SerializeField] protected float Decreasing;
    public int CurrBounceCount;

    void Bounce(Vector3 normal)
    {
        ++CurrBounceCount;
        projectileData.projectileType = EProjectileType.PT_Normal;

        projectileData.speed *= 1 - DecreasingRate;
        projectileData.speed = Mathf.Clamp(projectileData.speed - Decreasing, 0, float.MaxValue);
        m_Rigidbody.velocity = projectileData.velocity = Vector3.Reflect(m_Rigidbody.velocity, normal);
        projectileData.relativeForward = new Vector3(projectileData.velocity.x, 0, projectileData.velocity.z).normalized;
        projectileData.gravity = Vector3.up * projectileData.velocity.y;
        projectileData.accelerate = 0;
    }
    public override void Landed(Vector3 inLoc)
    {
        base.Landed(inLoc);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (IsGround(collision.collider))
        {
            Bounce(collision.contacts[0].normal);
        }
        else
        {
            base.OnCollisionEnter(collision);
        }
    }
    bool IsGround(Collider collider)
    {
        return CurrBounceCount < MaxBounceCount;
    }
}
