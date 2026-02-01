using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026
{
    public class MaskTooltip: RegularSingleton<MaskTooltip>
    {
        [Header("References")]
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private RectTransform m_RectTransform;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private GameObject[] m_StarVisuals;
        
        [Header("Settings")]
        [SerializeField] private Vector2 m_Offset = new Vector2(10f, -10f);
        [SerializeField] private float m_FadeDuration = 0.15f;

        private bool m_IsVisible;
        private float m_TargetAlpha;

        protected override void Awake()
        {
            base.Awake();
            Hide();
            m_CanvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (m_IsVisible)
            {
                UpdatePosition();
            }

            
            if (!Mathf.Approximately(m_CanvasGroup.alpha, m_TargetAlpha))
            {
                m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, m_TargetAlpha, Time.unscaledDeltaTime / m_FadeDuration);
            }
        }

        public void Show(Mask mask, int currentLevel)
        {
            m_IsVisible = true;
            m_TargetAlpha = 1f;
            gameObject.SetActive(true);
            
            m_NameText.text = mask.DisplayName;
            
            MaskBehaviour behaviour = Instantiate(mask.BehaviourPrefab);
            behaviour.Configure(mask, currentLevel + 1);
            m_DescriptionText.text = behaviour.GetFormattedDescription();
            Destroy(behaviour.gameObject);
            UpdatePosition();
        }

        public void Hide()
        {
            m_IsVisible = false;
            m_TargetAlpha = 0f;
        }

        private void SetupStars(int currentLevel)
        {
            if (m_StarVisuals == null) return;
            
            foreach (var star in m_StarVisuals)
            {
                if (star != null)
                    star.SetActive(false);
            }

            if (currentLevel == 0 && m_StarVisuals.Length > 0) 
                m_StarVisuals[0].SetActive(true);
            else if (currentLevel == 1 && m_StarVisuals.Length > 1) 
                m_StarVisuals[1].SetActive(true);
            else if (currentLevel >= 2 && m_StarVisuals.Length > 2) 
                m_StarVisuals[2].SetActive(true);
        }

        private void UpdatePosition()
        {
            Vector2 mousePosition = Input.mousePosition;
            
            if (m_Canvas && m_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    m_Canvas.transform as RectTransform,
                    mousePosition,
                    m_Canvas.worldCamera,
                    out Vector2 localPoint);
                
                m_RectTransform.localPosition = localPoint + m_Offset;
            }
            else
            {
                m_RectTransform.position = mousePosition + m_Offset;
            }

            // Clamp to screen bounds
            ClampToScreen();
        }

        private void ClampToScreen()
        {
            Vector3[] corners = new Vector3[4];
            m_RectTransform.GetWorldCorners(corners);

            float minX = corners[0].x;
            float maxX = corners[2].x;
            float minY = corners[0].y;
            float maxY = corners[2].y;

            Vector3 position = m_RectTransform.position;

            if (minX < 0)
                position.x -= minX;
            if (maxX > Screen.width)
                position.x -= (maxX - Screen.width);
            if (minY < 0)
                position.y -= minY;
            if (maxY > Screen.height)
                position.y -= (maxY - Screen.height);

            m_RectTransform.position = position;
        }
    }
}

