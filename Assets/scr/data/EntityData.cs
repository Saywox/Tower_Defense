using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
public class EntityData : ScriptableObject
{
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private EntityStats baseStats;

    public GameObject DeathEffect => deathEffect;
    public EntityStats BaseStats => baseStats;



}
