using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGJ2026
{
    public class UI_MaskEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_LevelText;

        private Mask m_Mask;
        private int m_Level;

        public void Setup(Mask mask)
        {
            m_Mask = mask;
            m_Icon.sprite = mask.Sprite;
        }

        public void SetLevel(int level)
        {
            m_Level = level;
            m_LevelText.text = $"lvl {level}";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_Mask != null)
            {
                MaskTooltip.Instance.Show(m_Mask, m_Level);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MaskTooltip.Instance.Hide();
        }
    }
}