using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;

namespace GGJ2026
{
    public abstract class MaskBehaviour : MonoBehaviour
    {
        public Mask Mask;
        public int Level;

        internal void Configure(Mask mask, int level)
        {
            Mask = mask;
            Level = Mathf.Clamp(level, 1, Mask.MaximumLevel);
        }

        public void IncreaseLevel()
        {
            var lastLevel = Level;
            Level = Mathf.Clamp(Level + 1, 1, Mask.MaximumLevel);

            if (lastLevel != Level)
            {
                OnLevelChange();
            }
        }

        public virtual void OnMaskAttached(in MaskAttachContext context)
        {
        }

        public virtual void OnLevelChange()
        {
        }
    }
}