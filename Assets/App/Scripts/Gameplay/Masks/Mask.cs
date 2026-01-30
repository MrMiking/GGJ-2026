using UnityEngine;

namespace GGJ2026
{
    [CreateAssetMenu(fileName = "New Mask", menuName = "GGJ2026/Mask")]
    public sealed class Mask : ScriptableObject
    {
        [field: SerializeField] public MaskBehaviour BehaviourPrefab { get; private set; }
    }
}