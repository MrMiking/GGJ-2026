using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace GGJ2026
{
    [DefaultExecutionOrder(-11)]
    public sealed class GameTimeline : RegularSingleton<GameTimeline>
    {
        public float GameTime { get; private set; }
        public bool IsPaused { get; private set; }

        private TimelineEvent m_CurrentTimelineEvent = TimelineEvent.None;
        public Action<TimelineEvent> OnTimelineEventChange;

        public TimelineEvent CurrentTimelineEvent {
            get => m_CurrentTimelineEvent;
            set
            {
                if (m_CurrentTimelineEvent == value)
                    return;

                m_CurrentTimelineEvent = value;
                OnTimelineEventChange?.Invoke(m_CurrentTimelineEvent);
            }
        }
        
        public void Pause()
        {
            IsPaused = true;
            MMTimeManager.Instance.UpdateTimescale = false;
            Time.timeScale = 0;
        }

        public void Resume()
        {
            IsPaused = false;
            MMTimeManager.Instance.UpdateTimescale = true;
            Time.timeScale = 1;
        }

        private void Update()
        {
            if (IsPaused)
                return;

            GameTime += Time.deltaTime;
        }
        
        public enum TimelineEvent
        {
            None,
            Introduction,
            Wave,
            Shop,
            Defeat,
        }
    }
}