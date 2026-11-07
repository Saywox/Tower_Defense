using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private const int SCOUT_INDEX = 0;
    private const int TROOPER_INDEX = 1;
    private const int TANK_INDEX = 2;
    private const string TEXT_TIMER = "Time until next wave: ";
    private const string TEXT_WAVE = "Wave ";

    [SerializeField] private EnemyWaveData[] waveDataObjects;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] invisibleWalls;
    [SerializeField] private GameObject[] enemyPrefabs;
    public Entity kingdom;
    private List<Enemy> currentlyAliveEnemies = new List<Enemy>();

    [SerializeField] private TextMeshProUGUI textWaveCount;
    [SerializeField] private TextMeshProUGUI textTimer;

    [SerializeField] private float waveTransitionDelay = 15f;
    [SerializeField] private float subwaveTransitionDelay = 5f;
    private float waveTransitionTimer;
    private bool duringWave = false;
    private bool duringSubwave = false;
    private int subwaveIndex = 0;
    private int maxSubwaves = 3;
    private int wavesPassed = 0;
    [SerializeField] private bool runTimer = true;

    private WaveType currentWaveType;
    private WaveType lastWaveType;

    [Header("Machine Learning Variables")]
    [SerializeField] private float distanceCheckRadius = 3f;
    [SerializeField] private LayerMask distanceCheckEntityLayer;
    public bool[] usedSpawnPoints = new bool[4];
    public float[] sideDistances = new float[4];
    public bool hasHitKingdom = false;
    private int waveDataIndex;

    [Header("Debugging")]
    [SerializeField] private int enemyIndex;
    [SerializeField] private int spawnIndex;

    public Entity Kingdom => kingdom;

    private void Awake()
    {
        Entity.OnDeath += OnEntityDeath;
        Entity.OnHealthChange += OnEntityHealthChange;
    }

    private void OnDestroy()
    {
        Entity.OnDeath -= OnEntityDeath;
        Entity.OnHealthChange -= OnEntityHealthChange;
    }

    private void Start()
    {
        for (int i = 0; i < sideDistances.Length; i++)
            sideDistances[i] = -1;
        for (int i = 0;i < invisibleWalls.Length; i++)
            invisibleWalls[i].SetActive(false);

        duringWave = false;
        duringSubwave = false;
        currentWaveType = WaveType.Recon;
        spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        waveTransitionTimer = waveTransitionDelay;
        textWaveCount.text = $"{TEXT_WAVE}{wavesPassed}";
    }

    private void Update()
    {
        HandleWaveTransitionTimer();
    }

    private void HandleWaveTransitionTimer()
    {
        if (!runTimer)
            return;

        if (waveTransitionTimer > 0)
        {
            textTimer.text = $"{TEXT_TIMER}{(int)waveTransitionTimer}";
            waveTransitionTimer -= Time.deltaTime;
        }
        else if (duringWave && !duringSubwave)
            StartSubwave();
        else if (!duringWave)
            StartWave();
    }

    private void OnEntityDeath(Entity entity)
    {
        Enemy enemy = entity as Enemy;
        if (currentlyAliveEnemies.Contains(enemy))
            currentlyAliveEnemies.Remove(enemy);

        if (currentlyAliveEnemies.Count == 0)
            FinishSubwave();

        if (currentWaveType != WaveType.Attack)
            return;

        Collider[] enemiesInProximity = Physics.OverlapSphere(kingdom.transform.position, distanceCheckRadius, distanceCheckEntityLayer);
        enemiesInProximity = Array.FindAll(enemiesInProximity, x => x.GetComponent<Entity>().allegiance != kingdom.allegiance);
        enemiesInProximity = enemiesInProximity.OrderBy(x => (x.transform.position - kingdom.transform.position).sqrMagnitude).ToArray();

        if (enemiesInProximity.Length == 0)
            return;

        float distanceToClosest = Vector3.Distance(enemiesInProximity[0].transform.position, kingdom.transform.position);
        if (sideDistances[spawnIndex] <= distanceToClosest)
            sideDistances[spawnIndex] = distanceToClosest;
    }

    private void OnEntityHealthChange(Entity victim, Entity origin)
    {
        if (currentWaveType != WaveType.Recon || victim != kingdom)
            return;

        hasHitKingdom = true;
    }

    private void StartWave()
    {
        subwaveIndex = 0;
        duringWave = true;
        StartSubwave();
        textWaveCount.text = $"{TEXT_WAVE}{wavesPassed}";
    }

    private void StartSubwave()
    {
        duringSubwave = true;
        SpawnSubwaveEnemies();
        for (int i = 0; i < invisibleWalls.Length; i++)
            invisibleWalls[i].SetActive(i != spawnIndex);
    }

    private void FinishSubwave()
    {
        lastWaveType = currentWaveType;
        if (currentWaveType == WaveType.Recon && hasHitKingdom)
            currentWaveType = WaveType.Attack;
        else
            GetNewSpawnIndex();
        if (currentWaveType == WaveType.Attack && !hasHitKingdom)
        {
            usedSpawnPoints[spawnIndex] = true;
            GetNewSpawnIndex();
            currentWaveType = WaveType.Recon;
        }
        duringSubwave = false;
        waveTransitionTimer = subwaveTransitionDelay;
        if (subwaveIndex == maxSubwaves)
        {
            FinishWave();
        }
        else
        {
            subwaveIndex++;
            StartSubwave();
        }
    }

    private void FinishWave()
    {
        waveTransitionTimer = waveTransitionDelay;
        duringWave = false;
        wavesPassed++;
        if (wavesPassed % 2 == 0)
        {
            for (int i = 0; i < sideDistances.Length; i++)
                sideDistances[i] = -1;
        }
    }

    private void SpawnSubwaveEnemies()
    {
        waveDataIndex = currentWaveType == WaveType.Recon ? 0 : 1;
        if (lastWaveType == WaveType.Attack)
            waveDataIndex = 2;

        for (int i = 0; i < waveDataObjects[waveDataIndex].ScoutCount; i++)
            SpawnEnemy(SCOUT_INDEX, spawnIndex);
        for (int i = 0; i < waveDataObjects[waveDataIndex].TrooperCount; i++)
            SpawnEnemy(TROOPER_INDEX, spawnIndex);
        for (int i = 0; i < waveDataObjects[waveDataIndex].TankCount; i++)
            SpawnEnemy(TANK_INDEX, spawnIndex);
    }

    private void GetNewSpawnIndex()
    {
        int newSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        bool hasVisitedAllSpawnPoints = true;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!spawnPoints[i])
            {
                hasVisitedAllSpawnPoints = false;
                break;
            }
        }
        if (!hasVisitedAllSpawnPoints)
        {
            while (spawnPoints[newSpawnIndex])
                newSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        }
        else
        {
            float[] distances = sideDistances.OrderBy(x => x).ToArray();
            newSpawnIndex = Array.IndexOf(sideDistances, distances[0]);
        }
        spawnIndex = newSpawnIndex;
    }

    private void SpawnEnemy(int enemyIndex, int spawnIndex)
    {
        GameObject enemyInstance = Instantiate(enemyPrefabs[enemyIndex], spawnPoints[spawnIndex].position, Quaternion.identity, transform);
        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        if (enemy.EnemyType == EnemyType.Scout ||
            enemy.EnemyType == EnemyType.Tank)
            enemy.target = kingdom;
        enemy.enemyManager = this;

        if (!currentlyAliveEnemies.Contains(enemy))
            currentlyAliveEnemies.Add(enemy);
    }

    [Button]
    public void Btn_SpawnEnemy()
    {
        SpawnEnemy(enemyIndex, spawnIndex);
    }

    [Button]
    public void Btn_KillSevenBillionPeople()
    {
        for (int i = currentlyAliveEnemies.Count - 1; i >= 0 ; i--)
        {
            currentlyAliveEnemies[i].ChangeHealth(currentlyAliveEnemies[i].stats.maxHealth, null);
        }
    }
}
