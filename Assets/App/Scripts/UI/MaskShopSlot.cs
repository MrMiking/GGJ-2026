using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGJ2026
{
    public class MaskShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private Image m_Icon;
        [SerializeField] private GameObject[] m_StarVisuals;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private GameObject m_LockOverlay;
        public Mask CurrentMask;

        private string m_Descriptor;
        private int m_CurrentLevel;

        public void Setup(Mask mask, int currentLevel, bool locked)
        {
            CurrentMask = mask;
            m_CurrentLevel = currentLevel;
            m_Icon.sprite = mask.Sprite;
            m_Icon.enabled = true;
            m_PriceText.enabled = true;
            m_PriceText.text = $"${mask.Price * mask.PricePerLevel[currentLevel]}";
            m_PriceText.color = locked ? Color.red : Color.black;
            m_LockOverlay.SetActive(locked);

            DisableStars();

            switch (CurrentMask.Rarity)
            {
                case MaskRarity.Epic:
                    m_StarVisuals[0].SetActive(true);
                    m_StarVisuals[1].SetActive(true);
                    m_StarVisuals[2].SetActive(true);
                    break;
                case MaskRarity.Rare:
                    m_StarVisuals[0].SetActive(true);
                    m_StarVisuals[1].SetActive(true);
                    break;
                case MaskRarity.Common:
                    m_StarVisuals[0].SetActive(true);
                    break;
            }
        }

        private void DisableStars()
        {
            foreach (var star in m_StarVisuals) star.SetActive(false);
        }
        
        public void Clear()
        {
            CurrentMask = null;
            m_CurrentLevel = 0;
            m_Icon.enabled = false;
            m_PriceText.enabled = false;
            foreach(var star in m_StarVisuals) star.SetActive(false);
            MaskTooltip.Instance.Hide();
            m_LockOverlay.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CurrentMask == null) return;
            MaskTooltip.Instance.Show(CurrentMask, m_CurrentLevel + 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MaskTooltip.Instance.Hide();
        }
    }
}