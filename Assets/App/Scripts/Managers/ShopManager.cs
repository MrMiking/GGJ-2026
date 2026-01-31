using System;
using UnityEngine;
using System.Collections.Generic;
using MVsToolkit.Dev;

namespace GGJ2026
{
    public class ShopManager : RegularSingleton<ShopManager>
    {
        [Header("Settings")]
        [SerializeField] private ShopPricing m_Pricing;

        [Header("References")]
        [SerializeField] private MaskInventory m_Inventory;
        [SerializeField] private MaskShopSlot[] m_Slots;
        [SerializeField] private MaskDatabase m_AvailableMaskPool;
        [SerializeField] private GameObject m_ShopPanel;
        
        private int m_RerollCount;
        
        [System.Serializable]
        public class ShopPricing
        {
            public int BaseRerollPrice = 5;
            public int PriceIncreasePerReroll = 2;
            private int m_RerollsDone = 0;

            public int GetCurrentPrice() => BaseRerollPrice + (m_RerollsDone * PriceIncreasePerReroll);
            public void Increment() => m_RerollsDone++;
        }

        private void Start()
        {
            CloseShop();
        }

        [Button]
        public void OpenShop()
        {
            GameStateManager.Instance.PushContext(GameState.Shop);
            m_ShopPanel.SetActive(true);
            SetupShop();
        }

        [Button]
        public void CloseShop()
        {
            GameStateManager.Instance.PopContext(GameState.Shop);
            m_ShopPanel.SetActive(false);
        }
        
        public void Reroll()
        {
            // TODO: Vérifier la monnaie ici
            SetupShop();
            m_Pricing.Increment();
        }

        private void SetupShop()
        {
            foreach (var slot in m_Slots)
            {
                var randomMask = m_AvailableMaskPool[UnityEngine.Random.Range(0, m_AvailableMaskPool.MaskCount)];
                
                int level = GetMaskLevelInInventory(randomMask);
                slot.Setup(randomMask, level);
            }
        }

        private void RefreshVisual()
        {
            foreach (var slot in m_Slots)
            {
                if (slot.CurrentMask == null) continue;
                int level = GetMaskLevelInInventory(slot.CurrentMask);
                slot.Setup(slot.CurrentMask, level);
                
                Debug.Log("---->"+slot.CurrentMask.name + level);
            }
        }
        
        private int GetMaskLevelInInventory(Mask mask)
        {
            for (int i = 0; i < MaskInventory.InventorySize; i++)
            {
                if (m_Inventory[i] == mask)
                {
                    return m_Inventory.GetMaskLevel(i);
                }
            }
            return 0;
        }

        public void OnSlotClicked(MaskShopSlot slot)
        {
            Mask mask = slot.CurrentMask;
            
            if (m_Inventory.TryGetMask(mask))
            {
                m_Inventory.IncreaseMaskLevel(mask);
                slot.Clear();
            }
            else if (m_Inventory.TryAddMask(mask))
            {
                slot.Clear();
            }
            else
            {
                Debug.Log("Inventory Full !");
            }
            
            RefreshVisual();
        }
    }
}