using MVsToolkit.Dev;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GGJ2026
{
    public class ShopManager : RegularSingleton<ShopManager>
    {
        [Header("Settings")]
        [SerializeField] private ShopPricing m_Pricing;

        [Header("References")]
        [SerializeField] private MaskShopSlot[] m_Slots;
        [SerializeField] private MaskDatabase m_AvailableMaskPool;
        [SerializeField] private GameObject m_ShopPanel;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        
        private int m_RerollCount;
        private MaskInventory m_Inventory => PlayerController.Instance.Inventory;

        public event Action OnCloseShop; 
        
        [System.Serializable]
        public class ShopPricing
        {
            public int BaseRerollPrice = 5;
            public int PriceIncreasePerReroll = 2;
            private int m_RerollsDone = 0;

            public int GetCurrentRerollPrice() => BaseRerollPrice + (m_RerollsDone * PriceIncreasePerReroll);
            public void Increment() => m_RerollsDone++;
            public void Reset() => m_RerollsDone = 0;
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
            m_Pricing.Reset();
            OnCloseShop?.Invoke();
        }
        
        public void Reroll()
        {
            if (GameManager.Instance.CurrentGold < m_Pricing.GetCurrentRerollPrice()) return;
            
            GameManager.Instance.CurrentGold -= m_Pricing.GetCurrentRerollPrice();
            m_Pricing.Increment();
            SetupShop();
        }

        private void SetupShop()
        {
            m_PriceText.text = $"Reroll ${m_Pricing.GetCurrentRerollPrice()}";

            var pool = m_AvailableMaskPool.GetMaskPoolForLevel(GameManager.Instance.Level);
            var maskListCopy = pool.masks.Clone();

            foreach (var slot in m_Slots)
            {
                var randomMask = maskListCopy.GetRandomAndRemove();
                if (randomMask == null)
                {
                    Debug.LogError($"Not enough mask in the pool for level {pool.gameLevel} !");
                }
                else
                {
                    int level = GetMaskLevelInInventory(randomMask);
                    slot.Setup(randomMask, level);
                }
            }
        }

        private void RefreshVisual()
        {
            foreach (var slot in m_Slots)
            {
                if (slot.CurrentMask != null)
                {
                    int level = GetMaskLevelInInventory(slot.CurrentMask);
                    slot.Setup(slot.CurrentMask, level);
                }
            }
        }
        
        private int GetMaskLevelInInventory(Mask mask)
        {
            return m_Inventory.GetMaskLevel(mask);
        }

        public void OnSlotClicked(MaskShopSlot slot)
        {
            Mask mask = slot.CurrentMask;

            if (mask == null) return;

            // Temporary for timoth�e to balance the game
            // Maybe change by locking the slot
            int level = GetMaskLevelInInventory(mask);
            int price = Mathf.CeilToInt(mask.Price * mask.PricePerLevel[level]);

            if (GameManager.Instance.CurrentGold < price)
            {
                Debug.Log("Not enough money !");
                return;
            }


            if (m_Inventory.TryGetMask(mask))
            {
                m_Inventory.IncreaseMaskLevel(mask);
                slot.Clear();
                GameManager.Instance.CurrentGold -= price;
            }
            else if (m_Inventory.TryAddMask(mask))
            {
                slot.Clear();
                GameManager.Instance.CurrentGold -= price;
            }
            else
            {
                Debug.Log("Inventory Full !");
            }
            
            RefreshVisual();
        }
    }
}