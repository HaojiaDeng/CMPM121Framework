using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public enum SpawnName
    {
        RED, GREEN, BONE
    }

    public SpawnName kind;

    // Add a type field - you can set this in the Inspector for each SpawnPoint GameObject
    public string spawnType = "default"; // e.g., "red", "bone", "green", or leave as "default"

    // Optional: Add gizmos for easier visualization in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = GetGizmoColor();
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawIcon(transform.position + Vector3.up * 0.6f, "SpawnPointIcon.png", true); // Requires an icon file
    }

    void OnDrawGizmosSelected()
    {
         Gizmos.color = Color.yellow;
         Gizmos.DrawWireSphere(transform.position, 0.6f);
    }

    private Color GetGizmoColor()
    {
        switch (spawnType.ToLower())
        {
            case "red": return Color.red;
            case "green": return Color.green;
            case "blue": return Color.blue; // Example if you add blue
            case "bone": return Color.white;
            default: return Color.gray;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
