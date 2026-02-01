using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GGJ2026
{
    public class GameSequencer: RegularSingleton<GameSequencer>
    {
        [Header("Settings")]
        [SerializeField] private float m_TimerBeforeShop = 10f;

        [SerializeField] private float m_TimeIntroduction;
        [Header("References")]
        [SerializeField] private InputActionReference m_InputActionReference;
        [SerializeField] private bool m_AutoStart = true;

        public float TimerBeforeShop => m_TimerBeforeShop;

        private IEnumerator Start()
        {
            if (!m_AutoStart)
            {
                PlayerController.Instance.gameObject.SetActive(false);
                
                yield return new WaitUntil(m_InputActionReference.action.WasPressedThisFrame);
                GameTimeline.Instance.CurrentTimelineEvent = GameTimeline.TimelineEvent.Introduction;
                yield return new WaitForSeconds(m_TimeIntroduction);
                
                PlayerController.Instance.gameObject.SetActive(true);
            }

            StartCoroutine(GameLoop());
        }
        
        private IEnumerator GameLoop()
        {
            while (GameTimeline.Instance.CurrentTimelineEvent != GameTimeline.TimelineEvent.Defeat)
            {
                yield return WaveLoop();
                yield return ShopLoop();
            }

            StartCoroutine(DefeatSequence());
        }

        private static IEnumerator ShopLoop()
        {
            // Shop Phase
            bool shopRunning = true;
            void ShopCloseGameTimeline() => shopRunning = false;
            ShopManager.Instance.OnCloseShop += ShopCloseGameTimeline;
            GameTimeline.Instance.CurrentTimelineEvent = GameTimeline.TimelineEvent.Shop;
            ShopManager.Instance.OpenShop();
            yield return new WaitWhile(()=> shopRunning);
            ShopManager.Instance.OnCloseShop -= ShopCloseGameTimeline;
        }

        private IEnumerator WaveLoop()
        {
            // Wave Phase
            GameManager.Instance.Level++;
            GameTimeline.Instance.CurrentTimelineEvent = GameTimeline.TimelineEvent.Wave;
            
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