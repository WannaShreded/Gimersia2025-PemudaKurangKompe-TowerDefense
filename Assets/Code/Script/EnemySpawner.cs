using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private float enemiesPerSecondCap = 15f;

    [Header("Objectives")]
    [SerializeField] private int goalWaves = 10;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();
    // Invoked whenever the current wave changes. Passes the new current wave as an int.
    public static UnityEvent<int> onWaveChanged = new UnityEvent<int>();

    [Header("UI References")]
    [SerializeField] private GameObject missionCompletePanel;

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private float eps;
    private bool isSpawning = false;

    private void Awake() {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start() {
        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(false);
        }
        StartCoroutine(StartWave());
    }

    private void Update() {
        if (!isSpawning) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / eps) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }
        
        if (enemiesAlive == 0 && enemiesLeftToSpawn ==0) {
            EndWave();
        }
    }

    private void EnemyDestroyed() {
        enemiesAlive--;
    }

    private IEnumerator StartWave() {
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        eps = EnemiesPerSecond();
    }

    private void EndWave() {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;

        // Notify listeners about the new wave number
        onWaveChanged.Invoke(currentWave);

        if (currentWave > goalWaves)
        {
            ShowMissionComplete();
        }
        else
        {
            StartCoroutine(StartWave());
        }
    }

    // Public read-only access for UI/other systems to query the current wave
    public int CurrentWave { get { return currentWave; } }

    private void ShowMissionComplete()
    {
        if (missionCompletePanel != null)
        {
            Time.timeScale = 0f;  // Pause the game
            missionCompletePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Mission Complete Panel not assigned in EnemySpawner!");
        }
    }
     
    private void SpawnEnemy() {
        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[index];
        Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
    
    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave, difficultyScalingFactor), 0f,enemiesPerSecondCap);
    }
}
