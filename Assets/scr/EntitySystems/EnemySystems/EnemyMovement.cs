using System;
using System.Collections.Generic;
using System.Linq;
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
        if (!HasPathToEntity(EnemyRef.target) && !EnemyRef.IsTargetWall())
        {
            EnemyRef.target = null;
            return;
        }

        if (NavMesh.SamplePosition(EnemyRef.target.transform.position, out NavMeshHit hit, 2, NavMesh.AllAreas))
            navMeshAgent.SetDestination(hit.position);

    }
    private void HandleTargetSearching()
    {
        if (EnemyRef.target != null) return;

        Collider[] entitiesInProximity = Physics.OverlapSphere(EnemyRef.transform.position, targetSearchRadius, entityLayer);
        entitiesInProximity = Array.FindAll(entitiesInProximity, x => x.GetComponent<Entity>().allegiance != EnemyRef.allegiance);
        entitiesInProximity = entitiesInProximity.OrderBy(x => (x.transform.position - EnemyRef.transform.position).sqrMagnitude).ToArray();

        if (TryGetKingdom(entitiesInProximity, out Entity kingdom))
        {
            if (kingdom != null && HasPathToEntity(kingdom))
            {
                EnemyRef.target = kingdom;
                return;
            }
        }

        foreach (Collider entityColl in entitiesInProximity)
        {
            if (!entityColl.TryGetComponent(out Entity entity))
                break;

            if (EnemyRef.IsTargetWall(entity))
            {
                EnemyRef.target = entity;
                break;
            }

            if (HasPathToEntity(entity))
            {
                EnemyRef.target = entity;
                break;
            }
        }
    }

    private bool TryGetKingdom(Collider[] entitiesInProximity, out Entity foundKingdom)
    {
        var entities = new List<Entity>();

        foreach (Collider coll in entitiesInProximity)
        {
            if (coll.TryGetComponent(out Entity s))
                entities.Add(s);
        }

        foundKingdom = entities.First(x => x == EnemyRef.enemyManager.kingdom);
        bool hasFoundKingdom = foundKingdom != null;
        return hasFoundKingdom;
    }

    private bool HasPathToEntity(Entity entity)
    {
        NavMeshPath path = new NavMeshPath();
        bool validPath = navMeshAgent.CalculatePath(entity.transform.position, path) && path.status == NavMeshPathStatus.PathComplete;
        return validPath;
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
