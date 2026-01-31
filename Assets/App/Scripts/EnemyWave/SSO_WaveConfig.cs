using UnityEngine;

[CreateAssetMenu(fileName = "SSO_WaveConfig", menuName = "SSO/Wave/SSO_WaveConfig")]
public class SSO_WaveConfig : ScriptableObject
{
    [Header("Settings")]
    public GameObject[] EnemiesPrefabs;
    public int EnemiesCount = 0;
    public WaveBurst[] Bursts;
    
    [System.Serializable]
    public struct WaveBurst
    {
        [Tooltip("In Seconds")]public int TimestampStart;
        public int BurstCount;
    }
}