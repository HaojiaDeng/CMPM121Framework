using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuSelectorController : MonoBehaviour
{
    public EnemySpawner spawner; // Set by EnemySpawner
    private LevelData levelData; // Store the level data for this button

    private Button button;
    private TextMeshProUGUI buttonText;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>(); // Assumes TextMeshPro

        if (button == null) { Debug.LogError("MenuSelectorController: Button component not found!", gameObject); return; }
        if (buttonText == null) { Debug.LogError("MenuSelectorController: TextMeshProUGUI child not found!", gameObject); }

        // Add listener to call OnLevelSelect when clicked
        button.onClick.RemoveAllListeners(); // Clear existing listeners from prefab/editor
        button.onClick.AddListener(OnLevelSelect);
    }

    // Called by EnemySpawner to set the level data and owner
    public void SetLevel(LevelData data, EnemySpawner owner)
    {
        this.levelData = data;
        this.spawner = owner;

        if (buttonText != null && levelData != null)
        {
            buttonText.text = levelData.name; // Set button text
        }
    }

    // Called when the button is clicked
    public void OnLevelSelect()
    {
        if (spawner != null && levelData != null)
        {
            spawner.StartLevel(levelData); // Tell the spawner to start this level
        }
        else
        {
            Debug.LogError("MenuSelectorController: Spawner or LevelData is null on click!", gameObject);
        }
    }
}
