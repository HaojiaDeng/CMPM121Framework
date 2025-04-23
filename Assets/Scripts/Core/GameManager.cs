using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json; // Required for JsonConvert
using System.IO; // Required for Path

public class GameManager
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
    }
    public GameState state;
    public int currentWave = 0; // Track current wave
    public int totalEnemiesThisWave = 0; // Total enemies expected in the current wave
    public int enemiesKilledThisWave = 0; // Enemies killed during the current wave
    public int playerHPAtLevelStart = -1;// Track player's hp when level start
    public int playerDamageTaken = 0;// Track damage taken
    public int playerDamageLastWave = 0;// Track damage taken in every wave
    public int playerHpAtWaveStart = -1;// Intialize value
    public string endMessage = ""; // Intialize endMessage


    public int countdown;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    // Loaded Data
    public List<EnemyData> enemyTypes { get; private set; }
    public List<LevelData> levels { get; private set; }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Remove(enemy))
        {
            enemiesKilledThisWave++; // Increment killed count for the wave
        }
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    private GameManager()
    {
        enemies = new List<GameObject>();
        LoadGameData(); // Load data on initialization
        state = GameState.PREGAME; // Start in pregame state
    }

    // Method to load data from JSON files in Resources folder
    private void LoadGameData()
    {
        try
        {
            TextAsset enemyJson = Resources.Load<TextAsset>("enemies");
            if (enemyJson == null) Debug.LogError("Failed to load enemies.json from Resources!");
            else enemyTypes = JsonConvert.DeserializeObject<List<EnemyData>>(enemyJson.text);

            TextAsset levelJson = Resources.Load<TextAsset>("levels");
             if (levelJson == null) Debug.LogError("Failed to load levels.json from Resources!");
            else levels = JsonConvert.DeserializeObject<List<LevelData>>(levelJson.text);

            // Ensure lists are not null even if loading failed
            if (enemyTypes == null) enemyTypes = new List<EnemyData>();
            if (levels == null) levels = new List<LevelData>();

            Debug.Log($"Loaded {enemyTypes.Count} enemy types and {levels.Count} levels.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading game data: {e.Message}\n{e.StackTrace}");
            // Initialize with empty lists to prevent null reference errors
             if (enemyTypes == null) enemyTypes = new List<EnemyData>();
             if (levels == null) levels = new List<LevelData>();
        }
    }

    // Helper to get enemy base data by name
    public EnemyData GetEnemyData(string name)
    {
        if (enemyTypes == null) return null;
        // Case-insensitive comparison
        return enemyTypes.FirstOrDefault(e => e.name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    // Reset game state for restarting or starting a new level
    public void ResetGame()
    {
        // Destroy existing enemies before clearing list
        if (enemies != null)
        {
            // Create a copy to iterate over while modifying the original list
            List<GameObject> enemiesToDestroy = new List<GameObject>(enemies);
            foreach (var enemy in enemiesToDestroy)
            {
                if (enemy != null) UnityEngine.Object.Destroy(enemy);
            }
            enemies.Clear();
        }
        else
        {
             enemies = new List<GameObject>();
        }

        currentWave = 0;
        totalEnemiesThisWave = 0; // Reset wave total
        enemiesKilledThisWave = 0; // Reset wave killed count
        state = GameState.PREGAME; // Set to PREGAME before level starts
        // Potentially reset player stats, projectiles etc. if needed
        if (projectileManager != null) projectileManager.ClearProjectiles();
    }
}
