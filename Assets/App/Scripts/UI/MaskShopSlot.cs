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
        public Mask CurrentMask;

        private string m_Descriptor;
        private int m_CurrentLevel;

        public void Setup(Mask mask, int currentLevel)
        {
            CurrentMask = mask;
            m_CurrentLevel = currentLevel;
            m_Icon.sprite = mask.Sprite;
            m_Icon.enabled = true;
            m_PriceText.enabled = true;
            m_PriceText.text = $"${mask.Price * mask.PricePerLevel[currentLevel]}";

            DisableStars();

            if (currentLevel == 0) m_StarVisuals[0].SetActive(true);
            if (currentLevel == 1) m_StarVisuals[1].SetActive(true);
            if(currentLevel >= 2) m_StarVisuals[2].SetActive(true);
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
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CurrentMask == null) return;
            MaskTooltip.Instance.Show(CurrentMask, m_CurrentLevel);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MaskTooltip.Instance.Hide();
        }
    }
}