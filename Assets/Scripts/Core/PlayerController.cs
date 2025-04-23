using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spellui;

    public int speed = 15;

    public Unit unit;
    public EnemySpawner enemySpawner;

    void Start()
    {
        unit = GetComponent<Unit>();
        if (unit == null) Debug.LogError("PlayerController requires Unit component!", gameObject);

        GameManager.Instance.player = gameObject;

        if (enemySpawner == null)
        {
            enemySpawner = FindAnyObjectByType<EnemySpawner>();
            if (enemySpawner == null) Debug.LogError("PlayerController could not find EnemySpawner in the scene!", gameObject);
        }

        if (healthui == null) Debug.LogError("PlayerController: healthui not assigned!", gameObject);
        if (manaui == null) Debug.LogError("PlayerController: manaui not assigned!", gameObject);
        if (spellui == null) Debug.LogError("PlayerController: spellui not assigned!", gameObject);
    }

    public void StartLevel()
    {
        int initialHp = 100;
        int initialMana = 125;
        int manaRegen = 8;

        spellcaster = new SpellCaster(initialMana, manaRegen, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());

        if (hp != null) { hp.OnDeath -= Die; }
        hp = new Hittable(initialHp, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;

        if (healthui != null) healthui.SetHealth(hp); else Debug.LogError("HealthUI not assigned!");
        if (manaui != null) manaui.SetSpellCaster(spellcaster); else Debug.LogError("ManaUI not assigned!");
        if (spellui != null && spellcaster.spell != null) spellui.SetSpell(spellcaster.spell); else Debug.LogError("SpellUI not assigned or default spell is null!");

        Debug.Log("Player Level Started: HP=" + hp.max_hp + ", Mana=" + spellcaster.max_mana);
    }

    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            if (unit != null) unit.movement = Vector2.zero;
        }
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state != GameManager.GameState.INWAVE && GameManager.Instance.state != GameManager.GameState.WAVEEND) return;
        if (spellcaster == null) return;

        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state != GameManager.GameState.INWAVE && GameManager.Instance.state != GameManager.GameState.WAVEEND)
        {
             if (unit != null) unit.movement = Vector2.zero;
             return;
        }
        if (unit == null) return;

        unit.movement = value.Get<Vector2>() * speed;
    }

    void Die()
    {
        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;

        Debug.Log("Player Died - Game Over");
        if (unit != null) unit.movement = Vector2.zero;

        if (enemySpawner != null)
        {
            enemySpawner.ShowGameOverScreen();
        }
        else
        {
            Debug.LogError("EnemySpawner reference not set on PlayerController. Cannot show Game Over screen.");
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        }
    }
}
