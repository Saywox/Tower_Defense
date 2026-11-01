using UnityEngine;

public class Defense : Entity
{
    [SerializeField] private DefensePlacement placement;

    public DefensePlacement Placement => placement;

    public bool isPlaced = false;

    private protected override void Initialize()
    {
        base.Initialize();
        placement.Initialize(this);
        placement.Select();
    }

    private protected override void Update()
    {
        if (isDead) return;

        Placement.Handle();

        if (!isPlaced)
            return;

        AttackHandler?.Handle();
        if (target != null)
        {
            Vector3 directionToTarget = target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
    }
}
