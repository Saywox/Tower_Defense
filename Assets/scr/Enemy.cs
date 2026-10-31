using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private EnemyMovement enemyMovement;
    public EnemyMovement EnemyMovement => enemyMovement;



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

}
