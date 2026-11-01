using UnityEngine;
using UnityEngine.InputSystem;

public class DefensePlacement : EntitySystem
{
    public Defense DefenseRef => entity as Defense;
    private DefenseManager manager;
    [SerializeField] private Vector3 objectSize = Vector3.one;
    private Quaternion defaultRotation;
    private Quaternion alternateRotation;
    private bool isRotated = false;
    public bool placeable = false;
    [SerializeField] private Collider[] objColliders;

    public override void Initialize(Entity entityRef)
    {
        entity = entityRef;
        DefenseRef.isPlaced = false;
        foreach (Collider collider in objColliders)
            collider.enabled = false;
    }
    public void Initialize(DefenseManager manager)
    {
        this.manager = manager;
        defaultRotation = Quaternion.identity;
        alternateRotation = Quaternion.identity * Quaternion.Euler(0, 90, 0);
    }

    public override void Handle()
    {
        if (!shouldUseSystem)
            return;

        HandlePlacement();
        HandlePlaceableColor();
    }

    private void HandlePlacement()
    {
        if (manager.currentlySelectedDefense != DefenseRef)
            return;

        Vector3 mousePos = Mouse.current.position.ReadValue();

        Ray ray = manager.cam.ScreenPointToRay(mousePos);

        bool hitObject = Physics.Raycast(ray, out RaycastHit hit, 100f, manager.groundLayer);
        if (!hitObject)
            return;

        float snappedX = Mathf.Round(hit.point.x / manager.gridSize) * manager.gridSize;
        float snappedZ = Mathf.Round(hit.point.z / manager.gridSize) * manager.gridSize;
        Vector3 worldPos = new Vector3(snappedX, 0, snappedZ);
        transform.position = worldPos;

        Collider[] overlappingObjects = Physics.OverlapBox(
            DefenseRef.transform.position, 
            objectSize / 2, 
            DefenseRef.transform.rotation, 
            manager.terrainObjectLayers);
        placeable = overlappingObjects.Length == 0;
    }

    public void Rotate()
    {
        if (DefenseRef.isPlaced)
            return;

        isRotated = !isRotated;
        DefenseRef.transform.rotation = isRotated ? alternateRotation : defaultRotation;
    }

    private void HandlePlaceableColor()
    {
        if (DefenseRef.isPlaced)
            return;

        // funny if™ structure: [condition] ? [if condition is true] : [if condition is false]
        DefenseRef.Visuals.SetMaterial(placeable ? manager.placeableMaterial : manager.unplaceableMaterial);
    }

    public void Select()
    {
        manager.currentlySelectedDefense = DefenseRef;
    }

    public void Place()
    {
        foreach (Collider collider in objColliders)
            collider.enabled = true;

        manager.currentlySelectedDefense = null;
        DefenseRef.isPlaced = true;
        DefenseRef.Visuals.SetMaterialsToDefault();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, objectSize);
    }
}
