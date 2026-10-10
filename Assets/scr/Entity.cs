using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("System variables")]
    [SerializeField] private EntityAttackHandler attackHandler;
    public EntityData data;
    public EntityStats stats;
    public Allegiance allegiance;
    public float health;
    public float atkSpeed;
    public bool isDead = false;
    public EntityAttackHandler AttackHandler => attackHandler;


    private void Start()
    {
        Initialize();

    }


    public void ChangeHealth(float healthChange) 
    {
        health -= healthChange;
        health = Mathf.Clamp(healthChange, 0, stats.maxHealth);
        if (health <= 0)
        {
            DestroyEntity();
        }
    }

    public virtual void DestroyEntity()
    {
        if (isDead) return;
        isDead = true;
        Instantiate(data.DeathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private protected virtual void Initialize()
    {
        stats = data.BaseStats;
        health = stats.maxHealth;
        attackHandler.Initialize(this);
    }
}
