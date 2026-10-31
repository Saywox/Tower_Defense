using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : EntitySystem
{
    public Enemy EnemyRef => entity as Enemy;
    [Header("Navigation Variables")]
    public NavMeshAgent navMeshAgent;

    [Header("Target Variables")]
    [SerializeField] private float targetSearchRadius = 10f;
    [SerializeField] private LayerMask entityLayer;
    public override void Initialize(Entity entityRef)
    {
        entity = entityRef;
        navMeshAgent.speed = EnemyRef.stats.mSpeed;
    }
    public override void Handle()
    {
        if (!shouldUseSystem) return;
        HandleMovement();
        HandleTargetSearching();

    }

    private void HandleMovement()
    {
        if (EnemyRef.target == null) return;
            navMeshAgent.SetDestination(EnemyRef.target.transform.position);

    }
    private void HandleTargetSearching()
    {
        if (EnemyRef.target != null) return;
        float closestEnemyDistance = Mathf.Infinity;
        Collider[] entitiesInProximity = Physics.OverlapSphere(EnemyRef.transform.position, targetSearchRadius, entityLayer);
        entitiesInProximity = Array.FindAll(entitiesInProximity, x => x.GetComponent<Entity>().allegiance != EnemyRef.allegiance);
        foreach (Collider entity in entitiesInProximity)
        {
            if (entity == null) continue;
            float distance = Vector3.Distance(EnemyRef.transform.position, entity.transform.position);
            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                EnemyRef.target = entity.GetComponent<Entity>();
            }


        }
    }


    public float DistanceToTarget()
    {
        return Vector3.Distance(entity.transform.position, entity.target.transform.position);

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, targetSearchRadius);
    }
}
