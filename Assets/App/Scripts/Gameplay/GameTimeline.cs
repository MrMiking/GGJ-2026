using UnityEngine;

namespace GGJ2026
{
    public sealed class GameTimeline : RegularSingleton<GameTimeline>
    {
        public float GameTime { get; private set; }
        public bool IsPaused { get; private set; }

        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0;
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1;
        }

        private void Update()
        {
            if (IsPaused)
                return;

            GameTime += Time.deltaTime;
        }
    }
}