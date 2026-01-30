using UnityEngine;

namespace GGJ2026
{
    public abstract class MaskBehaviour : MonoBehaviour
    {
        public int Level { get; private set; } = 0;

        public void IncreaseLevel()
        {
            Level++;
            OnLevelChange();
        }

        public virtual void OnMaskAttached(in MaskAttachContext context)
        {
        }

        public virtual void OnLevelChange()
        {
        }
    }
}