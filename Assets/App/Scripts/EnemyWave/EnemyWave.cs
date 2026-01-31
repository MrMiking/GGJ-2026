using System.Collections;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [SerializeField] private GameObject Enemy;

    [SerializeField] private float newWaveCooldown;

    public int waveCount = 0;
    public float spawnCooldown = 5f;
    public int currentEnemyAmount = 0;
    public int maxEnemyAmount = 0;
    public int minEnemyAmount = 0;
    public int enemiesPerSpawn = 1;
    public bool canSpawn = true;
    private Vector2 spawnArea;

    private void Start()
    {
        NewWave(1);
        StartCoroutine(SpawnCooldown());
    }

    private void Update()
    {
        if (currentEnemyAmount < minEnemyAmount)
        {
            SpawnEnemy();
        }
        else if (currentEnemyAmount > maxEnemyAmount)
        {
            canSpawn = false;
        }
        else
        {
            canSpawn = true;
        }
    }

    //Wave Management

    private IEnumerator NewWaveCooldown()
    {
        yield return new WaitForSeconds(newWaveCooldown);
        yield return new WaitUntil(() => canSpawn == true);
        NewWave(waveCount + 1);
    }

    private void NewWave(int i)
    {
        waveCount = i;
        switch (i)
        {
            case 1:
                minEnemyAmount = 3;
                maxEnemyAmount = 5;
                enemiesPerSpawn = 1;
                break;
            case 2:
                minEnemyAmount = 5;
                maxEnemyAmount = 8;
                enemiesPerSpawn = 2;
                break;
            case 3:
                minEnemyAmount = 8;
                maxEnemyAmount = 12;
                enemiesPerSpawn = 3;
                break;
            case 4:
                minEnemyAmount = 12;
                maxEnemyAmount = 16;
                enemiesPerSpawn = 4;
                break;
            case 5:
                minEnemyAmount = 16;
                maxEnemyAmount = 20;
                enemiesPerSpawn = 5;
                break;
            default:
                minEnemyAmount += 5;
                maxEnemyAmount += 5;
                enemiesPerSpawn += 1;
                break;
        }
        StartCoroutine(NewWaveCooldown());
    }


    //SPAWN LOOP
    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        yield return new WaitUntil(() => canSpawn == true);
        SpawnEnemies(enemiesPerSpawn);
    }

    private void SpawnEnemies(int enemiesAmount)
    {
        for (int i = 0; i < enemiesAmount; i++)
        {
            SpawnEnemy();
        }

        StartCoroutine(SpawnCooldown());
    }

    private void SpawnEnemy()
    {

        Vector2 spawnPosition = RandomSpawnPosition();

        GameObject newEnemy = Instantiate(Enemy);
        newEnemy.transform.position = spawnPosition;

        currentEnemyAmount++;
    }

    public void OnEnemyKilled()
    {
        currentEnemyAmount--;
    }

    private Vector2 RandomSpawnPosition()
    {
        Vector2 position = new Vector2();

        Vector2 playerPos = PlayerController.Instance.transform.position;
        spawnArea = new Vector2 (playerPos.x + Random.Range(25f, 30f), playerPos.y + Random.Range(12f, 15f));

        float f = Random.value > 0.5? -1f : 1f;
        if (Random.value > 0.5)
        {
            position.x = spawnArea.x * f;
            position.y = Random.Range(-spawnArea.y, spawnArea.y);
        }
        else
        {
            position.y = spawnArea.y * f;
            position.x = Random.Range(-spawnArea.x, spawnArea.x);
        }

        return position;
    }
}
