using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyType type;
    public EnemyManager enemyManager;

    public EnemyMovement EnemyMovement => enemyMovement;
    public EnemyType EnemyType => type;


    private protected override void Initialize()
    {
        base.Initialize();
        enemyMovement.Initialize(this);
    }

    private protected override void Update()
    {
        if (isDead) return;
        enemyMovement.Handle();
        AttackHandler.Handle();
    }

    public bool IsTargetWall(Entity potentialTarget = null)
    {
        if (target != null)
            potentialTarget = target;
        return potentialTarget is Defense && (potentialTarget as Defense).DefenseType == DefenseType.Wall;
    }
}
