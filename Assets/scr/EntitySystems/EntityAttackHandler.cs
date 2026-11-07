using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttackHandler : EntitySystem
{
    public float attackRadius = 0.5f;
    [SerializeField] private float attackTimer = 0;
    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private TargetPriority attackPrioriity;
    public Enemy EnemyRef => entity as Enemy;
    public override void Initialize(Entity entityRef)
    {
        entity = entityRef;
    }
    public override void Handle()
    {
        if (!shouldUseSystem) return;
        HandleAttackTimer();
    }

    private void HandleAttackTimer()
    {
        attackTimer -= Time.deltaTime * entity.stats.atkSpeed;
        if (attackTimer <= 0)
        {
            attackTimer = entity.stats.atkCoolDown;
            Attack();
        }
    }

    private void Attack()
    {
        //Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, entityLayer);
        //List<Entity> hitEntities = new List<Entity>();
        //AOE
        //foreach (Collider hit in hits)
        //{
        //    if (!hit.TryGetComponent(out Entity hitEntity)) continue;
        //    if (hitEntities .Contains(hitEntity)) continue;
        //    if (hitEntity.allegiance == entity.allegiance) continue;
        //    hitEntities.Add(hitEntity);
        //    hitEntity.ChangeHealth(entity.stats.dmg);
        //}

        if (entity.target == null)
            return;
        if (EnemyRef.EnemyMovement.DistanceToTarget() <= attackRadius)
            entity.target.ChangeHealth(entity.stats.dmg, entity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
