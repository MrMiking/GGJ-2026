using UnityEngine;

public class LootComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int ammount;
    
    [SerializeField] private float propulsionForce;
    [SerializeField] private float propulsionAngle;
    
    public void DropLoot()
    {
        for (int i = 0; i < Random.Range(ammount.x, ammount.y); i++)
        {
            
        }
    }
}