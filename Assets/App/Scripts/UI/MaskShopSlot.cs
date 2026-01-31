using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class MaskShopSlot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image m_Icon;
        [SerializeField] private GameObject[] m_StarVisuals;
        public Mask CurrentMask { get; private set; }

        public void Setup(Mask mask, int currentLevel)
        {
            CurrentMask = mask;
            m_Icon.sprite = mask.Sprite;
            m_Icon.enabled = true;

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
            m_Icon.enabled = false;
            foreach(var star in m_StarVisuals) star.SetActive(false);
        }
    }
}