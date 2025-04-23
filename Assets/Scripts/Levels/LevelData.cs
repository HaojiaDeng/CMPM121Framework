using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class SpawnData
{
    public string enemy;
    public string count = "1"; // RPN string, default to 1 if missing
    public string hp;    // RPN string (uses enemy base HP if missing)
    public string damage;// RPN string (uses enemy base damage if missing)
    public string speed; // RPN string (uses enemy base speed if missing)
    public float delay = 2.0f; // Default delay between spawns in this group
    public List<int> sequence; // Optional sequence timing (currently unused in spawner logic)
    public string location; // Optional spawn location specifier (currently unused in spawner logic)
}

[System.Serializable]
public class LevelData
{
    public string name;
    public int waves = -1; // -1 indicates endless
    public List<SpawnData> spawns;
}
