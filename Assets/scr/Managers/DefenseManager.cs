using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DefenseManager : MonoBehaviour
{
    private const string AVAILABLE_UNITS = "Available units: ";
    public Camera cam;
    [SerializeField] private GameObject[] defensePrefabs;

    public Defense currentlySelectedDefense;
    public LayerMask terrainObjectLayers;
    public LayerMask groundLayer;
    public Material placeableMaterial;
    public Material unplaceableMaterial;
    [SerializeField] private TextMeshProUGUI unitsText;
    [SerializeField] private Button[] defenseButtons;
    [SerializeField] private int availableUnits = 3;
    public float gridSize = 0.5f;

    private void Update()
    {
        HandlePlacementInputs();
    }

    private void HandlePlacementInputs()
    {
        if (currentlySelectedDefense == null)
            return;
        if (Mouse.current.leftButton.wasPressedThisFrame && 
            currentlySelectedDefense.Placement.placeable)
        {
            availableUnits--;
            unitsText.text = $"{AVAILABLE_UNITS}{availableUnits}";
            if (availableUnits == 0)
            {
                foreach (var button in defenseButtons)
                    button.interactable = false;
            }

            currentlySelectedDefense.Placement.Place();
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
            currentlySelectedDefense.Placement.Rotate();
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Destroy(currentlySelectedDefense.gameObject);
            currentlySelectedDefense = null;
        }
    }

    public void Btn_SpawnDefense(int index)
    {
        if (currentlySelectedDefense != null || availableUnits == 0)
            return;

        GameObject defenseinstance = Instantiate(defensePrefabs[index], Vector3.zero, Quaternion.identity);

        Defense defense = defenseinstance.GetComponent<Defense>();
        defense.Placement.Initialize(this);
    }
}
