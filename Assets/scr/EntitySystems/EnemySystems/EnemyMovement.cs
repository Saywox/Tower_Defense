using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : EntitySystem
{
    public Enemy EnemyRef => entity as Enemy;
    [Header("NavigationVariables")]
    public NavMeshAgent navMeshAgent;
    public Vector3 targetPos;
    public override void Initialize(Entity entityRef)
    {
       entity = entityRef;

    }
    public override void Handle()
    {
      if (!shouldUseSystem) return; 
    }
}
