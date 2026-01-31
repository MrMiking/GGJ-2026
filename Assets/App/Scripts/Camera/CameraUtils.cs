

using UnityEngine;

namespace GGJ2026
{
    public static class CameraUtils
    {
        
        public const float K_MarginExit = 0.1f;
        public const float K_KMarginEnter = 0.2f;
        public static bool IsWorldPositionVisible(Vector3 position, float margin)
        {
            if (Camera.main)
            {
                Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);
                return viewportPoint.x >= 0 - margin && viewportPoint.x <= 1 + margin &&
                       viewportPoint.y  >= 0 - margin && viewportPoint.y <= 1 + margin &&
                       viewportPoint.z > 0;
            }
            return false;
        }
    }
}