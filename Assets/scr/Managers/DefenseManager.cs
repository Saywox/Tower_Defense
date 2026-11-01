using UnityEngine;
using UnityEngine.InputSystem;

public class DefenseManager : MonoBehaviour
{
    public Camera cam;
    [SerializeField] private GameObject[] defensePrefabs;

    public Defense currentlySelectedDefense;
    public LayerMask terrainObjectLayers;
    public LayerMask groundLayer;
    public Material placeableMaterial;
    public Material unplaceableMaterial;
    public float gridSize = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlacementInputs();
    }

    private void HandlePlacementInputs()
    {
        if (currentlySelectedDefense == null)
            return;
        if (Mouse.current.leftButton.wasPressedThisFrame && 
            currentlySelectedDefense.Placement.placeable)
            currentlySelectedDefense.Placement.Place();
        if (Keyboard.current.rKey.wasPressedThisFrame)
            currentlySelectedDefense.Placement.Rotate();
    }

    public void Btn_SpawnDefense(int index)
    {
        if (currentlySelectedDefense != null)
            return;

        GameObject defenseinstance = Instantiate(defensePrefabs[index], Vector3.zero, Quaternion.identity);

        Defense defense = defenseinstance.GetComponent<Defense>();
        defense.Placement.Initialize(this);
    }
}
