using UnityEngine;

public abstract class EntitySystem : MonoBehaviour
{
public bool shouldUseSystem = true;

    [HideInInspector] public Entity entity;
    public abstract void Initialize(Entity entityRef);
    public abstract void Handle();
   
}
