using UnityEngine;
using TMPro;

public class WaveLabelController : MonoBehaviour
{
    TextMeshProUGUI tmp;
    private float waveStartTime = 0f;
    private float waveEndTime = 0f;
    private float waveDuration = 0f;
    private GameManager.GameState previousState;

    void Awake() // Use Awake for GetComponent
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogError("WaveLabelController requires a TextMeshProUGUI component!", gameObject);
            this.enabled = false; // Disable if component is missing
        }
    }

    void Start()
    {
        // Clear text initially or set default
        tmp.text = "";
    }

    void Update()
    {
        if (tmp == null) return; // Component missing
        if (GameManager.Instance == null) return; // GameManager not ready
        if (GameManager.Instance.state != previousState)// recording time between states of INWAVE&WAVEEND
        {
            if (previousState == GameManager.GameState.COUNTDOWN && GameManager.Instance.state == GameManager.GameState.INWAVE)
            {
                waveStartTime = Time.time;
            }

            if (previousState == GameManager.GameState.INWAVE && GameManager.Instance.state == GameManager.GameState.WAVEEND)
            {
                waveEndTime = Time.time;
                waveDuration = waveEndTime - waveStartTime;
            }

            previousState = GameManager.Instance.state;
        }



        switch (GameManager.Instance.state)
        {
            case GameManager.GameState.COUNTDOWN:
                // Use countdown value from GameManager, updated by EnemySpawner coroutine
                tmp.text = $"Wave {GameManager.Instance.currentWave} starting in {GameManager.Instance.countdown}...";
                break;
            case GameManager.GameState.INWAVE:
                // Show current wave and remaining enemies (Total - Killed)
                int enemiesRemaining = GameManager.Instance.totalEnemiesThisWave - GameManager.Instance.enemiesKilledThisWave;
                // Ensure remaining doesn't go below zero in display if something unexpected happens
                enemiesRemaining = Mathf.Max(0, enemiesRemaining);
                tmp.text = $"Wave {GameManager.Instance.currentWave} | Enemies Remaining: {enemiesRemaining}";
                break;
            case GameManager.GameState.WAVEEND:
                // Show wave complete message & time duration & damage taken in each wave
                int damageTaken = GameManager.Instance.playerDamageTaken;
                int damageThisWave = GameManager.Instance.playerDamageLastWave;
                tmp.text = $"Wave {GameManager.Instance.currentWave} Complete! Time: {waveDuration:F1} seconds Damage Taken This Wave: {damageThisWave}";
                break;
            case GameManager.GameState.PREGAME:
                tmp.text = "Select a Level"; // Or empty
                break;
            case GameManager.GameState.GAMEOVER:
                tmp.text = "";
                break;
            default:
                tmp.text = ""; // Clear for other states
                break;
        }
    }
}
