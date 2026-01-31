using UnityEngine;
using System.Collections.Generic;

namespace GGJ2026
{
    public class UI_MaskLists : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MaskInventory m_MaskInventory;
        [SerializeField] private GameObject m_MaskPrefab;
        [SerializeField] private Transform m_Container;
        
        private readonly Dictionary<Mask, UI_MaskEntry> m_Entries = new Dictionary<Mask, UI_MaskEntry>();

        private void OnEnable()
        {
            m_MaskInventory.OnMaskAttached += AddMask;
            m_MaskInventory.OnMaskRemoved += RemoveMask;
            m_MaskInventory.OnMaskLevelChanged += UpdateMaskLevel;
        }

        private void OnDisable()
        {
            m_MaskInventory.OnMaskAttached -= AddMask;
            m_MaskInventory.OnMaskRemoved -= RemoveMask;
            m_MaskInventory.OnMaskLevelChanged -= UpdateMaskLevel;
        }

        private void AddMask(Mask mask)
        {
            if (mask == null || m_Entries.ContainsKey(mask)) return;
            
            GameObject go = Instantiate(m_MaskPrefab, m_Container);
            if (go.TryGetComponent<UI_MaskEntry>(out var entry))
            {
                entry.Setup(mask);
                entry.SetLevel(m_MaskInventory.GetMaskLevel(mask));
                m_Entries.Add(mask, entry);
            }
        }

        private void RemoveMask(Mask mask)
        {
            if (m_Entries.TryGetValue(mask, out var entry))
            {
                Destroy(entry.gameObject);
                m_Entries.Remove(mask);
            }
        }

        private void UpdateMaskLevel(Mask mask, int level)
        {
            if (m_Entries.TryGetValue(mask, out var entry))
            {
                entry.SetLevel(level);
            }
        }
    }
}