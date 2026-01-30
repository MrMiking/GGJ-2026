using MVsToolkit.Dev;
using UnityEngine;

public class LootComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int ammount;
    [SerializeField] private float spawnAreaSize;
    
    [Header("References")]
    [SerializeField] private Loot lootPrefab;
    
    [Button]
    public void DropLoot()
    {
        int count = Random.Range(ammount.x, ammount.y);
        
        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnAreaSize;
            Vector3 spawnPos = transform.position + randomOffset;

            Loot loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnAreaSize);
    }
}