using UnityEngine;

public class EntityVisuals : EntitySystem
{
    [SerializeField] private MeshRenderer[] renderers;
    private Material[] defaultMaterials;

    public override void Initialize(Entity entityRef)
    {
        entity = entityRef;
        GetDefaultMaterials();
    }

    public override void Handle() { }

    private void GetDefaultMaterials()
    {
        defaultMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            defaultMaterials[i] = renderers[i].sharedMaterial;
    }

    public void SetMaterialsToDefault()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sharedMaterial = defaultMaterials[i];
    }

    public void SetMaterial(Material material)
    {
        foreach (var renderer in renderers)
            renderer.sharedMaterial = material;
    }
}
