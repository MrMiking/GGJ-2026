using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2026
{
    public class GameSequencer: RegularSingleton<GameSequencer>
    {
        [Header("Settings")]
        [SerializeField] private float m_TimerBeforeShop = 10f;
        [Header("References")]
        [SerializeField] private InputActionReference m_InputActionReference;
        [SerializeField] private bool m_AutoStart = true;

        public float TimerBeforeShop => m_TimerBeforeShop;
        
        private IEnumerator Start()
        {
            if (m_AutoStart == false)
            {
                yield return new WaitUntil(m_InputActionReference.action.WasPressedThisFrame);
                GameTimeline.Instance.CurrentTimelineEvent = GameTimeline.TimelineEvent.Introduction;
                yield return new WaitForSeconds(2f);
            }

            StartCoroutine(GameLoop());
        }
        
        private IEnumerator GameLoop()
        {
            while (GameTimeline.Instance.CurrentTimelineEvent != GameTimeline.TimelineEvent.Defeat)
            {
                // Wave Phase
                GameManager.Instance.Level++;
                GameTimeline.Instance.CurrentTimelineEvent = GameTimeline.TimelineEvent.Wave;
                yield return WaveLoop();
                
                // Shop Phase
                GameTimeline.Instance.CurrentTimelineEvent = GameTimeline.TimelineEvent.Shop;
                ShopManager.Instance.OpenShop();
                yield return new WaitWhile(()=> GameStateManager.Instance.CurrentState == GameState.Shop);
            }

            StartCoroutine(DefeatSequence());
        }

        private IEnumerator WaveLoop()
        {
            float timer = 0;
            
            while (timer < m_TimerBeforeShop)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        
        private IEnumerator DefeatSequence()
        {
            GameTimeline.Instance.CurrentTimelineEvent = GameTimeline.TimelineEvent.Defeat;
            yield return new WaitForSeconds(3f);
            // Handle defeat logic here (e.g., show defeat screen)
        }
    }
}