using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public static event Action<Entity> OnDeath;
    public static event Action<Entity, Entity> OnHealthChange;


    [Header("System variables")]
    [SerializeField] private EntityAttackHandler attackHandler;
    [SerializeField] private EntityVisuals visuals;
    public EntityData data;
    public Entity target;
    public EntityStats stats;
    public Allegiance allegiance;
    public float health;
    public bool isDead = false;
    public EntityAttackHandler AttackHandler => attackHandler;
    public EntityVisuals Visuals => visuals;


    private void Start()
    {
        Initialize();

    }


    private protected virtual void Update()
    {
        if (isDead) return;
        attackHandler?.Handle();
    }
    public void ChangeHealth(float healthChange, Entity origin) 
    {
        health -= healthChange;
        health = Mathf.Clamp(health, 0, stats.maxHealth);
        OnHealthChange?.Invoke(this, origin);
        if (health <= 0)
            DestroyEntity();
    }

    public virtual void DestroyEntity()
    {
        if (isDead) return;
        isDead = true;
        CallDeathEvent();
        if (data.DeathEffect != null)
            Instantiate(data.DeathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private protected virtual void Initialize()
    {
        stats = data.BaseStats;
        health = stats.maxHealth;
        attackHandler?.Initialize(this);
        visuals?.Initialize(this);
    }

    private protected void CallDeathEvent()
    {
        OnDeath?.Invoke(this);
    }
}
