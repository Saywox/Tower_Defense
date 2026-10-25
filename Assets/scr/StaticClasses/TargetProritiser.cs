using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetProritiser : MonoBehaviour
{
   public static Entity GetPrioritisedEntity(TargetPriority priority, Entity origin, List<Entity> entities)
    {
        return priority switch
        {
            TargetPriority.Closest => GetClosestEntity(origin, entities),
            TargetPriority.Farthest => GetFarthestEntity(origin, entities),
            TargetPriority.Tankiest => GetTankiestEntity(entities),
            TargetPriority.Squishiest => GetSquishiestEntity(origin, entities),
            _=>null
        };
    } 

    private static Entity GetClosestEntity(Entity origin, List<Entity> entities)
    {
        float closestEntityDistance = Mathf.Infinity;
        Entity closestEntity = null;

        foreach (Entity entity in entities)
        {
            if (entity == null) continue;

            float distance = Vector3.Distance(origin.transform.position, entity.transform.position);

            if (distance < closestEntityDistance)
            {
                closestEntityDistance = distance;
                closestEntity = entity;
            }
        }
        return closestEntity;
    }

    private static Entity GetFarthestEntity(Entity origin, List<Entity> entities)
    {
        float farthestEntityDistance = Mathf.NegativeInfinity;
        Entity farthestEntity = null;

        foreach (Entity entity in entities)
        {
            if (entity == null) continue;

            float distance = Vector3.Distance(origin.transform.position, entity.transform.position);

            if (distance > farthestEntityDistance)
            {
                farthestEntityDistance = distance;
                farthestEntity = entity;
            }
        }
        return farthestEntity;
    }

    private static Entity GetTankiestEntity(List<Entity> entities)
    {
        float highestEntityHP = Mathf.NegativeInfinity;
        Entity tankiestEntity = null;

        foreach (Entity entity in entities)
        {
            if (entity == null) continue;

            if (entity.stats.maxHealth > highestEntityHP)
            {
                highestEntityHP = entity.stats.maxHealth;
                tankiestEntity = entity;
            }
        }
        return tankiestEntity;
    }
     private static Entity GetSquishiestEntity(Entity origin, List<Entity> entities)
    {
        float lowestEntityHP = Mathf.Infinity;
        Entity squishiestEntity = null;

        foreach (Entity entity in entities)
        {
            if (entity == null) continue;

            if (entity.stats.maxHealth < lowestEntityHP)
            {
                lowestEntityHP = entity.stats.maxHealth;
                squishiestEntity = entity;
            }
        }
        return squishiestEntity;
    }

}
