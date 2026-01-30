using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class Loot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int goldAmount = 1;
    
    public void Collect(LootCollector target, float speed)
    {
        Tweener tweener = transform.DOMove(target.transform.position, speed).SetSpeedBased(true);

        tweener.OnUpdate(() =>
        {
            if (Vector3.Distance(transform.position, target.transform.position) > 1.0f)
            {
                tweener.ChangeEndValue(target.transform.position, true);
            }
        });

        tweener.OnComplete(OnCollected);
    }
    
    private void OnCollected()
    {
        GameManager.Instance.CurrentGold += goldAmount;
        Destroy(gameObject);
    }
}