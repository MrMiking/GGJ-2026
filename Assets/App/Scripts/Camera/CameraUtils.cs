

using UnityEngine;

namespace GGJ2026
{
    public static class CameraUtils
    {
        
        private const float Margin = 0.1f;
        public static bool IsWorldPositionVisible(Vector3 position)
        {
            if (Camera.main)
            {
                Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);
                return viewportPoint.x is >= 0 - Margin and <= 1 + Margin &&
                       viewportPoint.y is >= 0 - Margin and <= 1 + Margin &&
                       viewportPoint.z > 0;
            }
            return false;
        }
    }
}