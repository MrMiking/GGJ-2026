using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class UI_MaskEntry : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_LevelText;

        public void Setup(Mask mask)
        {
            m_Icon.sprite = mask.Sprite;
        }

        public void SetLevel(int level)
        {
            m_LevelText.text = $"lvl {level}";
        }
    }
}