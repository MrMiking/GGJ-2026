using System;
using UnityEngine;

namespace GGJ2026
{
    public sealed class MaskInventory : MonoBehaviour
    {
        public const int InventorySize = 5;

        [SerializeField] private Mask[] m_Masks = new Mask[InventorySize];

        [SerializeField] private MaskBehaviour[] m_Behaviours = new MaskBehaviour[InventorySize];
        
        public event Action<Mask> OnMaskAttached;
        public event Action<Mask> OnMaskRemoved;
        public event Action<Mask, int> OnMaskLevelChanged;

        private void Awake()
        {
            for (int i = 0; i < m_Masks.Length; i++)
            {
                if (m_Masks[i] != null)
                    AttachMaskBehaviour(i);
            }
        }

        public Mask this[int index] => m_Masks[index];

        public bool TryGetMask(Mask mask)
        {
            var index = Array.FindIndex(m_Masks, (m) => m == mask);
            if (index == -1)
                return false;

            return m_Masks[index];
        }

        public bool TryAddMask(Mask mask, int level = 1)
        {
            if (mask == null)
                return false;

            var firstEmptyIndex = Array.FindIndex(m_Masks, static (m) => m == null);
            if (firstEmptyIndex == -1)
            {
                return false;
            }

            m_Masks[firstEmptyIndex] = mask;
            AttachMaskBehaviour(firstEmptyIndex, level);

            OnMaskAttached?.Invoke(mask);
            return true;
        }

        public void RemoveMask(int index)
        {
            if (index < 0 || index >= InventorySize)
                return;

            var deletedMask = m_Masks[index];
            m_Masks[index] = null;
            ref var behaviour = ref m_Behaviours[index];
            
            if (behaviour == null) return;
            
            OnMaskRemoved?.Invoke(deletedMask);
            Destroy(behaviour.gameObject);
            behaviour = null;
        }

        public int GetMaskLevel(int index)
        {
            var behaviour = m_Behaviours[index];
            return behaviour == null ? 0 : behaviour.Level;
        }

        public int GetMaskLevel(Mask mask)
        {
            var index = Array.FindIndex(m_Masks, (m) => m == mask);
            if (index != -1)
                return GetMaskLevel(index);
            return 0;
        }

        public void IncreaseMaskLevel(int index)
        {
            var behaviour = m_Behaviours[index];
            if (behaviour)
            {
                behaviour.IncreaseLevel();
                OnMaskLevelChanged?.Invoke(this[index] ,GetMaskLevel(index));
            }
        }

        public void IncreaseMaskLevel(Mask mask)
        {
            var index = Array.FindIndex(m_Masks, (m) => m == mask);
            if (index != -1)
                IncreaseMaskLevel(index);
        }

        private void AttachMaskBehaviour(int index, int level = 1)
        {
            var mask = m_Masks[index];
            var maskBehaviour = Instantiate(mask.BehaviourPrefab, transform);
            m_Behaviours[index] = maskBehaviour;
            var ctx = new MaskAttachContext(this);
            maskBehaviour.Configure(mask, level);
            maskBehaviour.OnMaskAttached(ctx);
        }


#region Validation
        private void OnValidate()
        {
            if (m_Masks != null && m_Masks.Length == InventorySize)
                return;

            Array.Resize(ref m_Masks, InventorySize);
        }
#endregion
    }
}