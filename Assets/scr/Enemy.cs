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

    void Update()
    {
        if (isDead) return;
        enemyMovement.Handle();
    }

}
